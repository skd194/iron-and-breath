using System.Text.Json;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Domain.Enums;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Services;

public record VideoSeedEntry(
    string Exercise,
    string? YouTubeId,
    string? Title,
    string? ThumbnailUrl,
    int? DurationSeconds,
    string? Attribution);

public class VideoSeedFile
{
    public List<VideoSeedEntry> Videos { get; set; } = new();
}

/// <summary>
/// Upserts <see cref="ExerciseVideo"/> rows from a reviewable JSON file and
/// links them to exercises by name. Idempotent: fill ids, restart, re-run.
/// Entries with an empty id are skipped (exercise keeps its placeholder).
/// </summary>
public static class VideoSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static async Task SeedAsync(AppDbContext db, string filePath, ILogger? logger, CancellationToken ct = default)
    {
        if (!File.Exists(filePath))
        {
            logger?.LogInformation("Video seed file not found at {Path}; skipping video seeding.", filePath);
            return;
        }

        VideoSeedFile? file;
        try
        {
            await using var stream = File.OpenRead(filePath);
            file = await JsonSerializer.DeserializeAsync<VideoSeedFile>(stream, JsonOptions, ct);
        }
        catch (JsonException ex)
        {
            logger?.LogWarning(ex, "Video seed file at {Path} is not valid JSON; skipping.", filePath);
            return;
        }

        if (file is null || file.Videos.Count == 0)
        {
            return;
        }

        var exercises = await db.Exercises.ToListAsync(ct);
        var videos = await db.ExerciseVideos.ToListAsync(ct);

        var linked = 0;
        foreach (var entry in file.Videos)
        {
            if (string.IsNullOrWhiteSpace(entry.YouTubeId))
            {
                continue;
            }

            var id = entry.YouTubeId.Trim();
            var matches = exercises.Where(e => e.Name == entry.Exercise).ToList();
            if (matches.Count == 0)
            {
                logger?.LogWarning("Video seed references unknown exercise '{Name}'.", entry.Exercise);
                continue;
            }

            var video = videos.FirstOrDefault(v => v.Provider == VideoProvider.YouTube && v.ExternalId == id);
            if (video is null)
            {
                video = new ExerciseVideo { Provider = VideoProvider.YouTube, ExternalId = id };
                db.ExerciseVideos.Add(video);
                videos.Add(video);
            }

            video.Title = string.IsNullOrWhiteSpace(entry.Title) ? entry.Exercise : entry.Title!;
            video.ThumbnailUrl = entry.ThumbnailUrl;
            video.DurationSeconds = entry.DurationSeconds;
            video.Attribution = entry.Attribution;

            foreach (var exercise in matches)
            {
                exercise.Video = video;
                linked++;
            }
        }

        if (db.ChangeTracker.HasChanges())
        {
            await db.SaveChangesAsync(ct);
            logger?.LogInformation("Seeded videos: linked {Count} exercise(s).", linked);
        }
    }
}
