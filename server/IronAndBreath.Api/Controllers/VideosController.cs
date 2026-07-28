using IronAndBreath.Api.Dtos;
using IronAndBreath.Api.Services;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Controllers;

/// <summary>
/// The shared instructional-video library. Used by the workout config UI to
/// attach a clip to an exercise. Videos are global (not user-scoped).
/// </summary>
[ApiController]
[Authorize]
[Route("api/videos")]
public class VideosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IVideoResolver _videos;

    public VideosController(AppDbContext db, IVideoResolver videos)
    {
        _db = db;
        _videos = videos;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VideoLibraryItemDto>>> GetAll(CancellationToken ct)
    {
        var videos = await _db.ExerciseVideos.AsNoTracking().OrderBy(v => v.Title).ToListAsync(ct);
        return Ok(videos.Select(v => new VideoLibraryItemDto(v.Id, v.Title, _videos.Resolve(v))).ToList());
    }
}
