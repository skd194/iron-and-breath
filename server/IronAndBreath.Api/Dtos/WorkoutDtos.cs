namespace IronAndBreath.Api.Dtos;

/// <summary>Provider-agnostic video reference. Frontend never sees YouTube-specific fields.</summary>
public record VideoDto(
    string EmbedUrl,
    string? ThumbnailUrl,
    string Title,
    int? DurationSeconds,
    string? Attribution);

/// <summary>Breathing cues for the concentric/eccentric phases (null when not configured).</summary>
public record BreathingDto(string? Concentric, string? Eccentric, string? Notes);

public record ExerciseDto(
    int Id,
    string Name,
    int? TargetRepsLow,
    int? TargetRepsHigh,
    string RepsDisplay,
    string? Cue,
    int BaseSets,
    int SortOrder,
    VideoDto? Video,
    IReadOnlyList<string> PrimaryMuscles,
    IReadOnlyList<string> SecondaryMuscles,
    BreathingDto? Breathing,
    string? Tempo,
    string? Benefits,
    string? CommonMistakes,
    string? SafetyTips,
    string? AnimationRef);

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
