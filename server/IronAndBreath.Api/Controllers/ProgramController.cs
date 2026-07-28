using IronAndBreath.Api.Auth;
using IronAndBreath.Api.Dtos;
using IronAndBreath.Api.Services;
using IronAndBreath.Domain.Progression;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/program")]
public class ProgramController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly WarmUpOptions _warmUp;
    private readonly ICurrentUser _me;

    public ProgramController(AppDbContext db, IOptions<WarmUpOptions> warmUp, ICurrentUser me)
    {
        _db = db;
        _warmUp = warmUp.Value;
        _me = me;
    }

    /// <summary>Warm-up parameters (Surya Namaskar rounds and timings).</summary>
    [HttpGet("warmup")]
    public ActionResult<WarmUpDto> GetWarmUp()
        => Ok(new WarmUpDto(_warmUp.Name, _warmUp.Rounds, _warmUp.SecondsPerRound, _warmUp.TransitionSeconds));

    /// <summary>Current progression phase and its parameters, computed from today's date.</summary>
    [HttpGet("phase-today")]
    public async Task<ActionResult<PhaseTodayDto>> GetPhaseToday(CancellationToken ct)
    {
        var settings = await _db.UserProgramSettings.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == _me.Id, ct);
        var startDate = settings?.ProgramStartDate ?? DateOnly.FromDateTime(DateTime.Today);

        var phases = await _db.ProgramProgressionPhases.AsNoTracking().ToListAsync(ct);
        var today = DateOnly.FromDateTime(DateTime.Today);

        var phase = ProgressionCalculator.ResolvePhase(startDate, today, phases);
        var week = ProgressionCalculator.ResolveWeekNumber(startDate, today);

        return Ok(new PhaseTodayDto(
            phase.PhaseNumber,
            week,
            phase.RepsHintText,
            phase.SetDelta,
            phase.RestWorkSeconds,
            phase.RestBetweenSetsSeconds,
            phase.RestBetweenExercisesSeconds,
            phase.Notes,
            startDate));
    }
}
