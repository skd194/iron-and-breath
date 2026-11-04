using IronAndBreath.Api.Auth;
using IronAndBreath.Api.Dtos;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Domain.Progression;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions")]
public class SessionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;

    public SessionsController(AppDbContext db, ICurrentUser me)
    {
        _db = db;
        _me = me;
    }

    /// <summary>Session history, optionally filtered by an inclusive date range.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SessionDto>>> Get(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken ct)
    {
        var query = _db.WorkoutSessions.AsNoTracking()
            .Where(s => s.UserId == _me.Id)
            .Include(s => s.WorkoutDay)
            .Include(s => s.SetLogs)
            .AsQueryable();

        if (from is not null)
        {
            query = query.Where(s => s.Date >= from.Value);
        }
        if (to is not null)
        {
            query = query.Where(s => s.Date <= to.Value);
        }

        // Order by Id (monotonic with insertion) as the tiebreaker rather than
        // StartedAt — SQLite can't ORDER BY a DateTimeOffset, and Id keeps this
        // query portable to PostgreSQL too.
        var sessions = await query
            .OrderByDescending(s => s.Date)
            .ThenByDescending(s => s.Id)
            .ToListAsync(ct);

        return Ok(sessions.Select(ToDto).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SessionDto>> GetById(int id, CancellationToken ct)
    {
        var session = await _db.WorkoutSessions.AsNoTracking()
            .Include(s => s.WorkoutDay)
            .Include(s => s.SetLogs)
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == _me.Id, ct);

        return session is null ? NotFound() : Ok(ToDto(session));
    }

    /// <summary>
    /// Log a session — guided (from the player) or manual ("Log Previous Workout").
    /// WorkoutDayId is optional for ad-hoc manual sessions; set logs are optional.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SessionDto>> Create(CreateSessionRequest request, CancellationToken ct)
    {
        // Optional program day; when supplied it must belong to the user.
        if (request.WorkoutDayId is int dayId
            && !await _db.WorkoutDays.AnyAsync(d => d.Id == dayId && d.UserId == _me.Id, ct))
        {
            return ValidationProblem($"Workout day {dayId} does not exist.");
        }

        if (request.PerceivedDifficulty is int diff && diff is < 1 or > 10)
        {
            return ValidationProblem("Perceived difficulty must be between 1 and 10.");
        }

        var source = ParseSource(request.Source);

        // Validate + resolve any set logs against the user's own exercises.
        var setLogs = request.SetLogs ?? Array.Empty<CreateSetLogRequest>();
        var ownedExercises = setLogs.Any(sl => sl.ExerciseId is not null)
            ? await _db.Exercises
                .Where(e => e.WorkoutDay!.UserId == _me.Id)
                .ToDictionaryAsync(e => e.Id, e => e.Name, ct)
            : new Dictionary<int, string>();

        foreach (var sl in setLogs)
        {
            if (sl.ExerciseId is int exId && !ownedExercises.ContainsKey(exId))
            {
                return ValidationProblem($"Exercise {exId} does not exist.");
            }
            if (sl.Rpe is int rpe && rpe is < 1 or > 10)
            {
                return ValidationProblem("Set RPE must be between 1 and 10.");
            }
        }

        var phase = await ResolvePhaseForDateAsync(request.Date, ct);

        var session = new WorkoutSession
        {
            UserId = _me.Id,
            WorkoutDayId = request.WorkoutDayId,
            Date = request.Date,
            PhaseNumberAtCompletion = phase,
            StartedAt = request.StartedAt,
            CompletedAt = request.CompletedAt,
            Source = source,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            PerceivedDifficulty = request.PerceivedDifficulty,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        foreach (var sl in setLogs)
        {
            // Backfill a durable name from the linked exercise when not supplied.
            var name = string.IsNullOrWhiteSpace(sl.ExerciseName)
                ? (sl.ExerciseId is int id && ownedExercises.TryGetValue(id, out var n) ? n : null)
                : sl.ExerciseName.Trim();

            session.SetLogs.Add(new SessionSetLog
            {
                ExerciseId = sl.ExerciseId,
                ExerciseName = name,
                SetNumber = sl.SetNumber,
                RepsCompleted = sl.RepsCompleted,
                WeightKg = sl.WeightKg,
                Rpe = sl.Rpe,
                Notes = string.IsNullOrWhiteSpace(sl.Notes) ? null : sl.Notes.Trim(),
            });
        }

        _db.WorkoutSessions.Add(session);
        await _db.SaveChangesAsync(ct);

        // Reload with day for the response.
        await _db.Entry(session).Reference(s => s.WorkoutDay).LoadAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = session.Id }, ToDto(session));
    }

    /// <summary>Edit a mistakenly-logged session.</summary>
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<SessionDto>> Update(int id, UpdateSessionRequest request, CancellationToken ct)
    {
        var session = await _db.WorkoutSessions
            .Include(s => s.WorkoutDay)
            .Include(s => s.SetLogs)
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == _me.Id, ct);
        if (session is null)
        {
            return NotFound();
        }

        if (request.WorkoutDayId is int dayId
            && !await _db.WorkoutDays.AnyAsync(d => d.Id == dayId && d.UserId == _me.Id, ct))
        {
            return ValidationProblem($"Workout day {dayId} does not exist.");
        }
        if (request.PerceivedDifficulty is int diff && diff is < 1 or > 10)
        {
            return ValidationProblem("Perceived difficulty must be between 1 and 10.");
        }

        session.WorkoutDayId = request.WorkoutDayId;
        session.Date = request.Date;
        session.CompletedAt = request.CompletedAt;
        session.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        session.PerceivedDifficulty = request.PerceivedDifficulty;
        session.PhaseNumberAtCompletion = await ResolvePhaseForDateAsync(request.Date, ct);

        await _db.SaveChangesAsync(ct);
        await _db.Entry(session).Reference(s => s.WorkoutDay).LoadAsync(ct);

        return Ok(ToDto(session));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var session = await _db.WorkoutSessions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == _me.Id, ct);
        if (session is null)
        {
            return NotFound();
        }

        _db.WorkoutSessions.Remove(session);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private async Task<int> ResolvePhaseForDateAsync(DateOnly date, CancellationToken ct)
    {
        var settings = await _db.UserProgramSettings.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == _me.Id, ct);
        var startDate = settings?.ProgramStartDate ?? DateOnly.FromDateTime(DateTime.Today);
        var phases = await _db.ProgramProgressionPhases.AsNoTracking().ToListAsync(ct);
        return ProgressionCalculator.ResolvePhase(startDate, date, phases).PhaseNumber;
    }

    private static WorkoutSource ParseSource(string? source)
        => Enum.TryParse<WorkoutSource>(source, ignoreCase: true, out var parsed)
            ? parsed
            : WorkoutSource.Guided;

    private static SessionDto ToDto(WorkoutSession s) => new(
        s.Id,
        s.WorkoutDayId,
        s.WorkoutDay?.Name,
        s.Date,
        s.PhaseNumberAtCompletion,
        s.StartedAt,
        s.CompletedAt,
        s.Source.ToString(),
        s.Notes,
        s.PerceivedDifficulty,
        s.SetLogs
            .OrderBy(l => l.ExerciseId ?? int.MaxValue)
            .ThenBy(l => l.SetNumber)
            .Select(l => new SetLogDto(
                l.Id, l.ExerciseId, l.ExerciseName, l.SetNumber,
                l.RepsCompleted, l.WeightKg, l.Rpe, l.Notes))
            .ToList());
}
