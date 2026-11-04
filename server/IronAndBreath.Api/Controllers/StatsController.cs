using IronAndBreath.Api.Auth;
using IronAndBreath.Api.Dtos;
using IronAndBreath.Domain.Stats;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/stats")]
public class StatsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;

    public StatsController(AppDbContext db, ICurrentUser me)
    {
        _db = db;
        _me = me;
    }

    /// <summary>Aggregate stats for the dashboard (totals, streak, averages, per-day counts).</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<StatsSummaryDto>> GetSummary(CancellationToken ct)
    {
        var settings = await _db.UserProgramSettings.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == _me.Id, ct);
        var startDate = settings?.ProgramStartDate ?? DateOnly.FromDateTime(DateTime.Today);
        var target = settings?.DaysPerWeekTarget ?? 4;

        // Only completed sessions count towards stats.
        var completed = await _db.WorkoutSessions
            .AsNoTracking()
            .Where(s => s.UserId == _me.Id && s.CompletedAt != null)
            .Select(s => new { s.Date, s.WorkoutDayId })
            .ToListAsync(ct);

        var records = completed
            .Select(s => new CompletedSessionRecord(s.Date, s.WorkoutDayId))
            .ToList();

        var summary = StatsCalculator.Compute(records, DateOnly.FromDateTime(DateTime.Today), startDate, target);

        return Ok(new StatsSummaryDto(
            summary.TotalSessions,
            summary.SessionsThisWeek,
            summary.SessionsThisMonth,
            summary.MonthlyTarget,
            summary.CurrentWeeklyStreak,
            summary.AverageSessionsPerWeek,
            summary.PerDayCompletedCounts));
    }
}
