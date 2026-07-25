namespace IronAndBreath.Domain.Enums;

/// <summary>
/// Where an exercise's instructional video is served from.
/// Kept provider-agnostic so a self-hosted source can be added later
/// without touching the Exercise -> ExerciseVideo relationship.
/// </summary>
public enum VideoProvider
{
    YouTube = 0,
    SelfHosted = 1
}
