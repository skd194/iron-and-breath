namespace IronAndBreath.Api.Dtos;

/// <summary>Provider-agnostic video reference. Frontend never sees YouTube-specific fields.</summary>
public record VideoDto(
    string EmbedUrl,
    string? ThumbnailUrl,
    string Title,
    int? DurationSeconds,
    string? Attribution);

public record ExerciseDto(
    int Id,
    string Name,
    int? TargetRepsLow,
    int? TargetRepsHigh,
    string RepsDisplay,
    string? Cue,
    int BaseSets,
    int SortOrder,
    VideoDto? Video);

public record WorkoutDayDto(
    int Id,
    string Name,
    string Focus,
    int SortOrder,
    IReadOnlyList<ExerciseDto> Exercises);

public record PhaseTodayDto(
    int PhaseNumber,
    int WeekNumber,
    string RepsHintText,
    int SetDelta,
    int RestWorkSeconds,
    int RestBetweenSetsSeconds,
    int RestBetweenExercisesSeconds,
    string? Notes,
    DateOnly ProgramStartDate);

public record WarmUpDto(
    string Name,
    int Rounds,
    int SecondsPerRound,
    int TransitionSeconds);

public record SettingsDto(
    DateOnly ProgramStartDate,
    int DaysPerWeekTarget);

public record UpdateSettingsRequest(
    DateOnly ProgramStartDate,
    int DaysPerWeekTarget);
