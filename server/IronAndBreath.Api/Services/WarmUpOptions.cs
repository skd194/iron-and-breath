namespace IronAndBreath.Api.Services;

/// <summary>
/// Warm-up parameters, bound from the "WarmUp" configuration section so they
/// can be tuned without a redeploy (kept as config rather than a DB entity —
/// there is only ever one warm-up definition).
/// </summary>
public class WarmUpOptions
{
    public const string SectionName = "WarmUp";

    public string Name { get; set; } = "Surya Namaskar";
    public int Rounds { get; set; } = 4;
    public int SecondsPerRound { get; set; } = 50;
    public int TransitionSeconds { get; set; } = 15;
}
