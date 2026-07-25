namespace IronAndBreath.Api.Dtos;

public record SessionDto(
    int Id,
    int WorkoutDayId,
    string WorkoutDayName,
    DateOnly Date,
    int PhaseNumberAtCompletion,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt);

/// <summary>
/// Client posts what it knows; the server computes the progression phase from
/// the session date so that logic lives in one place.
/// </summary>
public record CreateSessionRequest(
    int WorkoutDayId,
    DateOnly Date,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt);

public record UpdateSessionRequest(
    int WorkoutDayId,
    DateOnly Date,
    DateTimeOffset? CompletedAt);
