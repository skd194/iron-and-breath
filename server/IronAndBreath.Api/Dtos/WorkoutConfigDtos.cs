namespace IronAndBreath.Api.Dtos;

public record CreateWorkoutDayRequest(string Name, string? Focus);

public record UpdateWorkoutDayRequest(string Name, string? Focus);

public record ReorderRequest(IReadOnlyList<int> OrderedIds);

public record UpsertExerciseRequest(
    string Name,
    string? RepsDisplay,
    int? TargetRepsLow,
    int? TargetRepsHigh,
    int BaseSets,
    string? Cue,
    int? VideoId);

/// <summary>An entry in the shared instructional-video library (for the config picker).</summary>
public record VideoLibraryItemDto(int Id, string Title, VideoDto Video);
