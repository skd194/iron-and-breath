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
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;

    public SettingsController(AppDbContext db, ICurrentUser me)
    {
        _db = db;
        _me = me;
    }

    /// <summary>Program settings (start date, weekly target).</summary>
    [HttpGet]
    public async Task<ActionResult<SettingsDto>> Get(CancellationToken ct)
    {
        var settings = await GetOrCreateAsync(ct);
        return Ok(new SettingsDto(settings.ProgramStartDate, settings.DaysPerWeekTarget));
    }

    /// <summary>Update the program start date / weekly target.</summary>
    [HttpPut]
    public async Task<ActionResult<SettingsDto>> Update(UpdateSettingsRequest request, CancellationToken ct)
    {
        if (request.DaysPerWeekTarget is < 1 or > 7)
        {
            return ValidationProblem("DaysPerWeekTarget must be between 1 and 7.");
        }

        var settings = await GetOrCreateAsync(ct);
        settings.ProgramStartDate = request.ProgramStartDate;
        settings.DaysPerWeekTarget = request.DaysPerWeekTarget;
        await _db.SaveChangesAsync(ct);

        return Ok(new SettingsDto(settings.ProgramStartDate, settings.DaysPerWeekTarget));
    }

    private async Task<UserProgramSettings> GetOrCreateAsync(CancellationToken ct)
    {
        var settings = await _db.UserProgramSettings.FirstOrDefaultAsync(s => s.UserId == _me.Id, ct);
        if (settings is null)
        {
            settings = new UserProgramSettings
            {
                UserId = _me.Id,
                ProgramStartDate = DateOnly.FromDateTime(DateTime.Today),
                DaysPerWeekTarget = 4
            };
            _db.UserProgramSettings.Add(settings);
            await _db.SaveChangesAsync(ct);
        }

        return settings;
    }
}
