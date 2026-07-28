using IronAndBreath.Api.Auth;
using IronAndBreath.Api.Dtos;
using IronAndBreath.Api.Services;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/workout-days")]
public class WorkoutDaysController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IVideoResolver _videos;
    private readonly ICurrentUser _me;

    public WorkoutDaysController(AppDbContext db, IVideoResolver videos, ICurrentUser me)
    {
        _db = db;
        _videos = videos;
        _me = me;
    }

    // ---- Reads ----

    /// <summary>The current user's workout days with their exercises and video references.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WorkoutDayDto>>> GetAll(CancellationToken ct)
    {
        var days = await OwnedDays()
            .AsNoTracking()
            .Include(d => d.Exercises)
                .ThenInclude(e => e.Video)
            .OrderBy(d => d.SortOrder)
            .ToListAsync(ct);

        return Ok(days.Select(d => d.ToDto(_videos)).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutDayDto>> GetById(int id, CancellationToken ct)
    {
        var day = await OwnedDays()
            .AsNoTracking()
            .Include(d => d.Exercises)
                .ThenInclude(e => e.Video)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        return day is null ? NotFound() : Ok(day.ToDto(_videos));
    }

    // ---- Day CRUD ----

    [HttpPost]
    public async Task<ActionResult<WorkoutDayDto>> CreateDay(CreateWorkoutDayRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ValidationProblem("Day name is required.");
        }

        var nextOrder = await OwnedDays().Select(d => (int?)d.SortOrder).MaxAsync(ct) ?? 0;
        var day = new WorkoutDay
        {
            UserId = _me.Id,
            Name = request.Name.Trim(),
            Focus = request.Focus?.Trim() ?? string.Empty,
            SortOrder = nextOrder + 1,
        };
        _db.WorkoutDays.Add(day);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = day.Id }, day.ToDto(_videos));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WorkoutDayDto>> UpdateDay(int id, UpdateWorkoutDayRequest request, CancellationToken ct)
    {
        var day = await OwnedDays().Include(d => d.Exercises).ThenInclude(e => e.Video)
            .FirstOrDefaultAsync(d => d.Id == id, ct);
        if (day is null)
        {
            return NotFound();
        }
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ValidationProblem("Day name is required.");
        }

        day.Name = request.Name.Trim();
        day.Focus = request.Focus?.Trim() ?? string.Empty;
        await _db.SaveChangesAsync(ct);

        return Ok(day.ToDto(_videos));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDay(int id, CancellationToken ct)
    {
        var day = await OwnedDays().FirstOrDefaultAsync(d => d.Id == id, ct);
        if (day is null)
        {
            return NotFound();
        }

        // WorkoutSession -> WorkoutDay is Restrict; block deletion if history exists.
        if (await _db.WorkoutSessions.AnyAsync(s => s.WorkoutDayId == id, ct))
        {
            return Conflict(new { message = "This day has logged sessions and can't be deleted. Delete those sessions first." });
        }

        _db.WorkoutDays.Remove(day);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>Reorder days by supplying their ids in the desired order.</summary>
    [HttpPost("reorder")]
    public async Task<IActionResult> ReorderDays(ReorderRequest request, CancellationToken ct)
    {
        var days = await OwnedDays().ToListAsync(ct);
        ApplyOrder(days, d => d.Id, (d, order) => d.SortOrder = order, request.OrderedIds);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // ---- Exercise CRUD (nested under a day) ----

    [HttpPost("{dayId:int}/exercises")]
    public async Task<ActionResult<WorkoutDayDto>> CreateExercise(int dayId, UpsertExerciseRequest request, CancellationToken ct)
    {
        var day = await OwnedDays().Include(d => d.Exercises).FirstOrDefaultAsync(d => d.Id == dayId, ct);
        if (day is null)
        {
            return NotFound();
        }

        var validation = await ValidateExerciseAsync(request, ct);
        if (validation is not null)
        {
            return validation;
        }

        var nextOrder = day.Exercises.Count == 0 ? 0 : day.Exercises.Max(e => e.SortOrder);
        day.Exercises.Add(new Exercise
        {
            Name = request.Name.Trim(),
            RepsDisplay = request.RepsDisplay?.Trim() ?? string.Empty,
            TargetRepsLow = request.TargetRepsLow,
            TargetRepsHigh = request.TargetRepsHigh,
            BaseSets = request.BaseSets,
            Cue = string.IsNullOrWhiteSpace(request.Cue) ? null : request.Cue.Trim(),
            VideoId = request.VideoId,
            SortOrder = nextOrder + 1,
        });

        await _db.SaveChangesAsync(ct);
        return await ReloadDay(dayId, ct);
    }

    [HttpPut("{dayId:int}/exercises/{exerciseId:int}")]
    public async Task<ActionResult<WorkoutDayDto>> UpdateExercise(int dayId, int exerciseId, UpsertExerciseRequest request, CancellationToken ct)
    {
        var day = await OwnedDays().Include(d => d.Exercises).FirstOrDefaultAsync(d => d.Id == dayId, ct);
        var exercise = day?.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (day is null || exercise is null)
        {
            return NotFound();
        }

        var validation = await ValidateExerciseAsync(request, ct);
        if (validation is not null)
        {
            return validation;
        }

        exercise.Name = request.Name.Trim();
        exercise.RepsDisplay = request.RepsDisplay?.Trim() ?? string.Empty;
        exercise.TargetRepsLow = request.TargetRepsLow;
        exercise.TargetRepsHigh = request.TargetRepsHigh;
        exercise.BaseSets = request.BaseSets;
        exercise.Cue = string.IsNullOrWhiteSpace(request.Cue) ? null : request.Cue.Trim();
        exercise.VideoId = request.VideoId;

        await _db.SaveChangesAsync(ct);
        return await ReloadDay(dayId, ct);
    }

    [HttpDelete("{dayId:int}/exercises/{exerciseId:int}")]
    public async Task<IActionResult> DeleteExercise(int dayId, int exerciseId, CancellationToken ct)
    {
        var day = await OwnedDays().Include(d => d.Exercises).FirstOrDefaultAsync(d => d.Id == dayId, ct);
        var exercise = day?.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (day is null || exercise is null)
        {
            return NotFound();
        }

        _db.Exercises.Remove(exercise);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{dayId:int}/exercises/reorder")]
    public async Task<IActionResult> ReorderExercises(int dayId, ReorderRequest request, CancellationToken ct)
    {
        var day = await OwnedDays().Include(d => d.Exercises).FirstOrDefaultAsync(d => d.Id == dayId, ct);
        if (day is null)
        {
            return NotFound();
        }

        var list = day.Exercises.ToList();
        ApplyOrder(list, e => e.Id, (e, order) => e.SortOrder = order, request.OrderedIds);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // ---- Helpers ----

    private IQueryable<WorkoutDay> OwnedDays() => _db.WorkoutDays.Where(d => d.UserId == _me.Id);

    private async Task<ActionResult<WorkoutDayDto>> ReloadDay(int dayId, CancellationToken ct)
    {
        var reloaded = await OwnedDays()
            .AsNoTracking()
            .Include(d => d.Exercises).ThenInclude(e => e.Video)
            .FirstAsync(d => d.Id == dayId, ct);
        return Ok(reloaded.ToDto(_videos));
    }

    private async Task<ActionResult?> ValidateExerciseAsync(UpsertExerciseRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ValidationProblem("Exercise name is required.");
        }
        if (request.BaseSets is < 1 or > 12)
        {
            return ValidationProblem("Base sets must be between 1 and 12.");
        }
        if (request.VideoId is int vid && !await _db.ExerciseVideos.AnyAsync(v => v.Id == vid, ct))
        {
            return ValidationProblem($"Video {vid} does not exist.");
        }
        return null;
    }

    private static void ApplyOrder<T>(List<T> items, Func<T, int> idOf, Action<T, int> setOrder, IReadOnlyList<int> orderedIds)
    {
        var order = 1;
        foreach (var id in orderedIds)
        {
            var item = items.FirstOrDefault(i => idOf(i) == id);
            if (item is not null)
            {
                setOrder(item, order++);
            }
        }
        // Any ids not referenced keep a stable order after the explicit ones.
        foreach (var item in items.Where(i => !orderedIds.Contains(idOf(i))))
        {
            setOrder(item, order++);
        }
    }
}
