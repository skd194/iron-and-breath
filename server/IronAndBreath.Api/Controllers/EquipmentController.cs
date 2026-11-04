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
[Route("api/equipment")]
public class EquipmentController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;

    public EquipmentController(AppDbContext db, ICurrentUser me)
    {
        _db = db;
        _me = me;
    }

    /// <summary>
    /// The user's equipment. If they have none yet (e.g. an account created before
    /// the inventory existed), the seeded template is lazily cloned in so existing
    /// accounts are populated on first load.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EquipmentItemDto>>> GetAll(CancellationToken ct)
    {
        await EnsureSeededAsync(ct);
        var items = await Owned().AsNoTracking().OrderBy(e => e.SortOrder).ToListAsync(ct);
        return Ok(items.Select(ToDto).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<EquipmentItemDto>> Create(UpsertEquipmentRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ValidationProblem("Equipment name is required.");
        }

        var nextOrder = await Owned().Select(e => (int?)e.SortOrder).MaxAsync(ct) ?? 0;
        var item = new EquipmentItem { UserId = _me.Id, SortOrder = nextOrder + 1 };
        Apply(item, request);
        _db.EquipmentItems.Add(item);
        await _db.SaveChangesAsync(ct);
        return Ok(ToDto(item));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EquipmentItemDto>> Update(int id, UpsertEquipmentRequest request, CancellationToken ct)
    {
        var item = await Owned().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (item is null)
        {
            return NotFound();
        }
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ValidationProblem("Equipment name is required.");
        }

        Apply(item, request);
        await _db.SaveChangesAsync(ct);
        return Ok(ToDto(item));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await Owned().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (item is null)
        {
            return NotFound();
        }
        _db.EquipmentItems.Remove(item);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("reorder")]
    public async Task<IActionResult> Reorder(ReorderRequest request, CancellationToken ct)
    {
        var items = await Owned().ToListAsync(ct);
        var order = 1;
        foreach (var itemId in request.OrderedIds)
        {
            var item = items.FirstOrDefault(i => i.Id == itemId);
            if (item is not null) item.SortOrder = order++;
        }
        foreach (var item in items.Where(i => !request.OrderedIds.Contains(i.Id)))
        {
            item.SortOrder = order++;
        }
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // ---- Helpers ----

    private IQueryable<EquipmentItem> Owned() => _db.EquipmentItems.Where(e => e.UserId == _me.Id);

    private async Task EnsureSeededAsync(CancellationToken ct)
    {
        if (await Owned().AnyAsync(ct))
        {
            return;
        }
        var templates = await _db.EquipmentItems.AsNoTracking()
            .Where(e => e.UserId == null)
            .OrderBy(e => e.SortOrder)
            .ToListAsync(ct);
        foreach (var t in templates)
        {
            _db.EquipmentItems.Add(new EquipmentItem
            {
                UserId = _me.Id,
                Name = t.Name,
                Category = t.Category,
                Specs = t.Specs,
                Status = t.Status,
                SortOrder = t.SortOrder,
            });
        }
        if (templates.Count > 0)
        {
            await _db.SaveChangesAsync(ct);
        }
    }

    private static void Apply(EquipmentItem item, UpsertEquipmentRequest request)
    {
        item.Name = request.Name.Trim();
        item.Specs = string.IsNullOrWhiteSpace(request.Specs) ? null : request.Specs.Trim();
        item.Category = Enum.TryParse<EquipmentCategory>(request.Category, true, out var cat) ? cat : EquipmentCategory.Other;
        item.Status = Enum.TryParse<EquipmentStatus>(request.Status, true, out var st) ? st : EquipmentStatus.Owned;
    }

    private static EquipmentItemDto ToDto(EquipmentItem e)
        => new(e.Id, e.Name, e.Category.ToString(), e.Specs, e.Status.ToString(), e.SortOrder);
}
