using IronAndBreath.Api.Dtos;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/muscles")]
public class MusclesController : ControllerBase
{
    private readonly AppDbContext _db;

    public MusclesController(AppDbContext db) => _db = db;

    /// <summary>The global muscle reference catalog (what each muscle does + injury notes).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MuscleDto>>> GetAll(CancellationToken ct)
    {
        var muscles = await _db.Muscles.AsNoTracking().OrderBy(m => m.Name).ToListAsync(ct);
        return Ok(muscles.Select(m => new MuscleDto(m.Id, m.Name, m.Group, m.Function, m.InjuryNotes)).ToList());
    }
}
