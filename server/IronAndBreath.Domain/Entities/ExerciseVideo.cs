using IronAndBreath.Domain.Enums;

namespace IronAndBreath.Domain.Entities;

/// <summary>
/// Instructional video metadata for an exercise. The backend never stores or
/// proxies the video itself — only enough metadata to resolve an embed URL.
/// </summary>
public class ExerciseVideo
{
    public int Id { get; set; }

    public VideoProvider Provider { get; set; }

    /// <summary>Provider-specific identifier (e.g. a YouTube video id). Null for fully-qualified <see cref="Url"/> sources.</summary>
    public string? ExternalId { get; set; }

    /// <summary>Absolute URL for self-hosted sources; null when <see cref="ExternalId"/> is used.</summary>
    public string? Url { get; set; }

    public string? ThumbnailUrl { get; set; }

    public int? DurationSeconds { get; set; }

    public string Title { get; set; } = string.Empty;

    /// <summary>Attribution / source note for the clip (channel, license, etc.).</summary>
    public string? Attribution { get; set; }

    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}
