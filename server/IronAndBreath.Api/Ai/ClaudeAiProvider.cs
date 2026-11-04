using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace IronAndBreath.Api.Ai;

/// <summary>
/// Claude-backed provider. Calls the Anthropic Messages API with streaming and
/// re-emits text deltas. Uses raw HTTP (a typed HttpClient) so the server can
/// forward Server-Sent Events straight through to the browser without pulling in
/// the SDK's evolving streaming types; the wire contract is the documented
/// /v1/messages SSE format.
/// </summary>
public class ClaudeAiProvider : IAiProvider
{
    private const string Endpoint = "https://api.anthropic.com/v1/messages";
    private const string AnthropicVersion = "2023-06-01";

    private readonly HttpClient _http;
    private readonly AiOptions _options;
    private readonly ILogger<ClaudeAiProvider> _logger;

    public ClaudeAiProvider(HttpClient http, IOptions<AiOptions> options, ILogger<ClaudeAiProvider> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public string Name => $"Claude ({_options.Model})";

    public async IAsyncEnumerable<string> StreamReplyAsync(
        string systemPrompt,
        IReadOnlyList<AiTurn> history,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var payload = new
        {
            model = _options.Model,
            max_tokens = _options.MaxTokens,
            system = systemPrompt,
            stream = true,
            messages = history.Select(t => new { role = t.Role, content = t.Content }).ToArray(),
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"),
        };
        request.Headers.Add("x-api-key", _options.ApiKey);
        request.Headers.Add("anthropic-version", AnthropicVersion);

        using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Claude API error {Status}: {Body}", (int)response.StatusCode, body);
            throw new InvalidOperationException($"AI provider returned {(int)response.StatusCode}.");
        }

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync(ct)) is not null)
        {
            if (!line.StartsWith("data:", StringComparison.Ordinal))
            {
                continue;
            }
            var data = line[5..].Trim();
            if (data.Length == 0)
            {
                continue;
            }

            var text = ExtractTextDelta(data);
            if (text is { Length: > 0 })
            {
                yield return text;
            }
        }
    }

    /// <summary>Pulls the text out of a content_block_delta SSE line, or null.</summary>
    private static string? ExtractTextDelta(string data)
    {
        try
        {
            using var doc = JsonDocument.Parse(data);
            var root = doc.RootElement;
            if (root.TryGetProperty("type", out var type) && type.GetString() == "content_block_delta"
                && root.TryGetProperty("delta", out var delta)
                && delta.TryGetProperty("type", out var deltaType) && deltaType.GetString() == "text_delta"
                && delta.TryGetProperty("text", out var textEl))
            {
                return textEl.GetString();
            }
        }
        catch (JsonException)
        {
            // Ignore keep-alive / non-JSON lines.
        }
        return null;
    }
}
