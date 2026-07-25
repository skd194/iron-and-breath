using IronAndBreath.Api.Dtos;
using IronAndBreath.Api.Services;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Route("api/workout-days")]
public class WorkoutDaysController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IVideoResolver _videos;

    public WorkoutDaysController(AppDbContext db, IVideoResolver videos)
    {
        _db = db;
        _videos = videos;
    }

    /// <summary>All workout days with their exercises and video references.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WorkoutDayDto>>> GetAll(CancellationToken ct)
    {
        var days = await _db.WorkoutDays
            .AsNoTracking()
            .Include(d => d.Exercises)
                .ThenInclude(e => e.Video)
            .OrderBy(d => d.SortOrder)
            .ToListAsync(ct);

        return Ok(days.Select(d => d.ToDto(_videos)).ToList());
    }

    /// <summary>A single workout day by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutDayDto>> GetById(int id, CancellationToken ct)
    {
        var day = await _db.WorkoutDays
            .AsNoTracking()
            .Include(d => d.Exercises)
                .ThenInclude(e => e.Video)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        return day is null ? NotFound() : Ok(day.ToDto(_videos));
    }
}
