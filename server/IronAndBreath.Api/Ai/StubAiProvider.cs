using System.Runtime.CompilerServices;

namespace IronAndBreath.Api.Ai;

/// <summary>
/// Local fallback used when no API key is configured. Runs a lightweight scripted
/// coach so the chat is fully functional in development without a key — and so the
/// streaming/persistence path can be exercised end-to-end. Swapped for
/// <see cref="ClaudeAiProvider"/> automatically once CLAUDE_API_KEY is set.
/// </summary>
public class StubAiProvider : IAiProvider
{
    public string Name => "Coach (offline demo)";

    public async IAsyncEnumerable<string> StreamReplyAsync(
        string systemPrompt,
        IReadOnlyList<AiTurn> history,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var reply = Compose(history);
        foreach (var chunk in reply.Split(' '))
        {
            ct.ThrowIfCancellationRequested();
            yield return chunk + " ";
            await Task.Delay(18, ct); // simulate token streaming
        }
    }

    /// <summary>
    /// A tiny discovery script that advances through profile questions based on how
    /// many times the user has spoken, then points them at the configurator.
    /// </summary>
    private static string Compose(IReadOnlyList<AiTurn> history)
    {
        var userTurns = history.Count(t => t.Role == "user");
        return userTurns switch
        {
            <= 1 =>
                "Great to meet you! I'm your training coach. To build the right plan, tell me your main goal — "
                + "are you after muscle size, strength, fat loss, or general fitness?",
            2 =>
                "Love it. What's your experience level — beginner, intermediate, or advanced — and how many "
                + "days per week can you realistically train?",
            3 =>
                "Perfect. What equipment do you have access to (bodyweight only, dumbbells, full gym), and roughly "
                + "how long do you want each session to be?",
            4 =>
                "Thanks — that gives me a clear picture. A solid starting split for you is an upper/lower rotation "
                + "with compound lifts first, 3 working sets of 8-12 reps, progressing weekly. "
                + "Head to the Configure page to set up these days, then start a guided session from the dashboard. "
                + "Want me to refine any day, or adjust volume?",
            _ =>
                "Good question. Keep progressing by adding a rep or a little weight each week, prioritise sleep and "
                + "protein, and log your sessions so we can review them together. "
                + "(Set CLAUDE_API_KEY to unlock the full AI coach — this is the offline demo responder.)",
        };
    }
}
