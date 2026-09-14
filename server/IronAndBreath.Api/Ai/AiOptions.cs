namespace IronAndBreath.Api.Ai;

/// <summary>
/// AI provider configuration, bound from the "Ai" section. The API key is
/// supplied via config or the CLAUDE_API_KEY environment variable — never
/// hardcoded. Empty key disables live AI and falls back to a local stub.
/// </summary>
public class AiOptions
{
    public const string SectionName = "Ai";

    /// <summary>Anthropic API key. Config "Ai:ApiKey" or env CLAUDE_API_KEY.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Claude model id. Defaults to the latest Opus; override for cost/speed.</summary>
    public string Model { get; set; } = "claude-opus-4-8";

    /// <summary>Max output tokens per reply.</summary>
    public int MaxTokens { get; set; } = 2048;

    /// <summary>True when a real key is configured (otherwise the stub provider runs).</summary>
    public bool Enabled => !string.IsNullOrWhiteSpace(ApiKey);
}
