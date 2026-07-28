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
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == _me.Id, ct);

        return session is null ? NotFound() : Ok(ToDto(session));
    }

    /// <summary>Log a completed (or abandoned, if CompletedAt is null) session.</summary>
    [HttpPost]
    public async Task<ActionResult<SessionDto>> Create(CreateSessionRequest request, CancellationToken ct)
    {
        if (!await _db.WorkoutDays.AnyAsync(d => d.Id == request.WorkoutDayId && d.UserId == _me.Id, ct))
        {
            return ValidationProblem($"Workout day {request.WorkoutDayId} does not exist.");
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
            CreatedAt = DateTimeOffset.UtcNow,
        };

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
        var session = await _db.WorkoutSessions.Include(s => s.WorkoutDay)
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == _me.Id, ct);
        if (session is null)
        {
            return NotFound();
        }

        if (!await _db.WorkoutDays.AnyAsync(d => d.Id == request.WorkoutDayId && d.UserId == _me.Id, ct))
        {
            return ValidationProblem($"Workout day {request.WorkoutDayId} does not exist.");
        }

        session.WorkoutDayId = request.WorkoutDayId;
        session.Date = request.Date;
        session.CompletedAt = request.CompletedAt;
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

    private static SessionDto ToDto(WorkoutSession s) => new(
        s.Id,
        s.WorkoutDayId,
        s.WorkoutDay?.Name ?? string.Empty,
        s.Date,
        s.PhaseNumberAtCompletion,
        s.StartedAt,
        s.CompletedAt);
}
