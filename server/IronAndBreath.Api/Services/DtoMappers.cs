using IronAndBreath.Api.Dtos;
using IronAndBreath.Domain.Entities;

namespace IronAndBreath.Api.Services;

/// <summary>
/// Entity -> DTO mapping. Video resolution is delegated to an
/// <see cref="IVideoProvider"/> so the frontend only ever sees a
/// provider-agnostic <see cref="VideoDto"/> (milestone 5 adds real providers).
/// </summary>
public static class DtoMappers
{
    public static WorkoutDayDto ToDto(this WorkoutDay day, IVideoResolver videos)
        => new(
            day.Id,
            day.Name,
            day.Focus,
            day.SortOrder,
            day.Exercises
                .OrderBy(e => e.SortOrder)
                .Select(e => e.ToDto(videos))
                .ToList());

    public static ExerciseDto ToDto(this Exercise exercise, IVideoResolver videos)
        => new(
            exercise.Id,
            exercise.Name,
            exercise.TargetRepsLow,
            exercise.TargetRepsHigh,
            exercise.RepsDisplay,
            exercise.Cue,
            exercise.BaseSets,
            exercise.SortOrder,
            exercise.Video is null ? null : videos.Resolve(exercise.Video));
}
