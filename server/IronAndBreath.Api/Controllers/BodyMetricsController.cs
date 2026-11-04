using IronAndBreath.Api.Auth;
using IronAndBreath.Api.Dtos;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/body-metrics")]
public class BodyMetricsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;

    public BodyMetricsController(AppDbContext db, ICurrentUser me)
    {
        _db = db;
        _me = me;
    }

    /// <summary>Bodyweight log, oldest first, optionally filtered by date range.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BodyMetricDto>>> Get(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken ct)
    {
        var query = Owned();
        if (from is not null) query = query.Where(m => m.Date >= from.Value);
        if (to is not null) query = query.Where(m => m.Date <= to.Value);

        var items = await query.AsNoTracking().OrderBy(m => m.Date).ThenBy(m => m.Id).ToListAsync(ct);
        return Ok(items.Select(m => new BodyMetricDto(m.Id, m.Date, m.WeightKg)).ToList());
    }

    /// <summary>Log (or overwrite) the weight for a date. Updates the profile's current weight.</summary>
    [HttpPost]
    public async Task<ActionResult<BodyMetricDto>> Log(LogBodyMetricRequest request, CancellationToken ct)
    {
        if (request.WeightKg is <= 0 or > 500)
        {
            return ValidationProblem("Weight must be between 0 and 500 kg.");
        }

        var metric = await Owned().FirstOrDefaultAsync(m => m.Date == request.Date, ct);
        if (metric is null)
        {
            metric = new BodyMetric { UserId = _me.Id, Date = request.Date };
            _db.BodyMetrics.Add(metric);
        }
        metric.WeightKg = request.WeightKg;
        await _db.SaveChangesAsync(ct);

        // Keep the nutrition profile's weight in sync with the most recent entry.
        var latest = await Owned().OrderByDescending(m => m.Date).ThenByDescending(m => m.Id).FirstAsync(ct);
        var profile = await _db.NutritionProfiles.FirstOrDefaultAsync(p => p.UserId == _me.Id, ct);
        if (profile is not null && latest.Id == metric.Id)
        {
            profile.WeightKg = metric.WeightKg;
            await _db.SaveChangesAsync(ct);
        }

        return Ok(new BodyMetricDto(metric.Id, metric.Date, metric.WeightKg));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var metric = await Owned().FirstOrDefaultAsync(m => m.Id == id, ct);
        if (metric is null)
        {
            return NotFound();
        }
        _db.BodyMetrics.Remove(metric);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private IQueryable<BodyMetric> Owned() => _db.BodyMetrics.Where(m => m.UserId == _me.Id);
}
