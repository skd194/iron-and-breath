namespace IronAndBreath.Domain.Entities;

/// <summary>How a <see cref="WorkoutSession"/> was recorded.</summary>
public enum WorkoutSource
{
    /// <summary>Performed live in the app's guided player.</summary>
    Guided = 0,

    /// <summary>Entered after the fact via "Log Previous Workout".</summary>
    Manual = 1,

    /// <summary>Brought in from an external source (future).</summary>
    Imported = 2,
}
