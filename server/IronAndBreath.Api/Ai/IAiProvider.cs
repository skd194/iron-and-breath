namespace IronAndBreath.Api.Ai;

/// <summary>One message in the conversation passed to the AI provider.</summary>
public record AiTurn(string Role, string Content);

/// <summary>
/// Abstraction over the AI backend so the app isn't coupled to Claude. Streams
/// the assistant reply token-by-token. Swap implementations (Claude, future
/// providers, per-user keys) without touching the controller/UI.
/// </summary>
public interface IAiProvider
{
    /// <summary>Human-readable provider name (surfaced to the client).</summary>
    string Name { get; }

    /// <summary>Streams the assistant's reply given the system prompt + prior turns.</summary>
    IAsyncEnumerable<string> StreamReplyAsync(
        string systemPrompt,
        IReadOnlyList<AiTurn> history,
        CancellationToken ct);
}
