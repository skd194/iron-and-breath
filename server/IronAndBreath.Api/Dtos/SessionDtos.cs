namespace IronAndBreath.Api.Dtos;

/// <summary>A single logged set within a session.</summary>
public record SetLogDto(
    int Id,
    int? ExerciseId,
    string? ExerciseName,
    int SetNumber,
    int? RepsCompleted,
    decimal? WeightKg,
    int? Rpe,
    string? Notes);

public record SessionDto(
    int Id,
    int? WorkoutDayId,
    string? WorkoutDayName,
    DateOnly Date,
    int PhaseNumberAtCompletion,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    string Source,
    string? Notes,
    int? PerceivedDifficulty,
    IReadOnlyList<SetLogDto> SetLogs);

/// <summary>A set to persist as part of a session (create).</summary>
public record CreateSetLogRequest(
    int? ExerciseId,
    string? ExerciseName,
    int SetNumber,
    int? RepsCompleted,
    decimal? WeightKg,
    int? Rpe,
    string? Notes);

/// <summary>
/// Client posts what it knows; the server computes the progression phase from
/// the session date so that logic lives in one place. WorkoutDayId is optional
/// for ad-hoc manual sessions. Source defaults to Guided when omitted.
/// </summary>
public record CreateSessionRequest(
    int? WorkoutDayId,
    DateOnly Date,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    string? Source = null,
    string? Notes = null,
    int? PerceivedDifficulty = null,
    IReadOnlyList<CreateSetLogRequest>? SetLogs = null);

public record UpdateSessionRequest(
    int? WorkoutDayId,
    DateOnly Date,
    DateTimeOffset? CompletedAt,
    string? Notes = null,
    int? PerceivedDifficulty = null);
