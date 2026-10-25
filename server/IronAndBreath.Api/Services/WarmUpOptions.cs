namespace IronAndBreath.Api.Services;

/// <summary>A single dynamic warm-up movement performed before the main work.</summary>
public class WarmUpMovement
{
    public string Name { get; set; } = string.Empty;
    public int Seconds { get; set; }
    public string? Cue { get; set; }
}

/// <summary>
/// Warm-up parameters, bound from the "WarmUp" configuration section so they
/// can be tuned without a redeploy (kept as config rather than a DB entity —
/// there is only ever one warm-up definition). A short dynamic-movement routine
/// runs first to get the body ready, followed by the Surya Namaskar flow.
/// </summary>
public class WarmUpOptions
{
    public const string SectionName = "WarmUp";

    public string Name { get; set; } = "Surya Namaskar";
    public int Rounds { get; set; } = 4;
    public int SecondsPerRound { get; set; } = 50;
    public int TransitionSeconds { get; set; } = 15;

    /// <summary>Dynamic warm-up movements shown before the Surya Namaskar flow.</summary>
    public List<WarmUpMovement> Movements { get; set; } = new()
    {
        new() { Name = "Arm Circles", Seconds = 30, Cue = "Big slow circles — forwards, then backwards." },
        new() { Name = "Shoulder Rolls", Seconds = 30, Cue = "Roll the shoulders back and down to open the chest." },
        new() { Name = "Bodyweight Squats", Seconds = 40, Cue = "Slow and full-depth to open the hips and knees." },
        new() { Name = "Leg Swings", Seconds = 40, Cue = "Swing each leg front-to-back, then side-to-side." },
        new() { Name = "Cat-Cow", Seconds = 30, Cue = "Mobilise the spine with your breath." },
        new() { Name = "Light Skipping", Seconds = 60, Cue = "Easy pace on the balls of the feet to raise the heart rate." },
    };
}
