using IronAndBreath.Api.Dtos;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Domain.Enums;

namespace IronAndBreath.Api.Services;

/// <summary>
/// Turns a stored <see cref="ExerciseVideo"/> into a provider-agnostic
/// <see cref="VideoDto"/>. This is the seam the milestone-5 self-hosted
/// provider plugs into — the frontend contract never changes.
/// </summary>
public interface IVideoResolver
{
    VideoDto Resolve(ExerciseVideo video);
}

public class VideoResolver : IVideoResolver
{
    public VideoDto Resolve(ExerciseVideo video)
    {
        var embedUrl = video.Provider switch
        {
            VideoProvider.YouTube => $"https://www.youtube-nocookie.com/embed/{video.ExternalId}",
            VideoProvider.SelfHosted => video.Url ?? string.Empty,
            _ => string.Empty
        };

        return new VideoDto(
            embedUrl,
            video.ThumbnailUrl,
            video.Title,
            video.DurationSeconds,
            video.Attribution);
    }
}
