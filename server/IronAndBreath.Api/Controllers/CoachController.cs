using System.Text.Json;
using IronAndBreath.Api.Ai;
using IronAndBreath.Api.Auth;
using IronAndBreath.Api.Dtos;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/coach")]
public class CoachController : ControllerBase
{
    // The coach persona. Kept server-side so it can evolve without client changes.
    private const string SystemPrompt =
        "You are an expert personal fitness coach inside the Iron & Breath app. "
        + "Your job is to understand the user's goals and constraints and help them build and adjust a workout plan. "
        + "Ask focused follow-up questions (one or two at a time) to learn their goal, desired physique, experience "
        + "level, training days per week, available equipment, session length, target muscles, and any restrictions. "
        + "Build a structured picture progressively rather than repeating questions. When you have enough, propose a "
        + "concrete weekly plan (days, exercises, sets, reps, rest) and tell them they can set it up on the Configure "
        + "page and start a guided session from the dashboard. Be concise, encouraging, and practical. Respond directly "
        + "without preamble.";

    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;
    private readonly IAiProvider _ai;
    private readonly AiOptions _aiOptions;

    public CoachController(AppDbContext db, ICurrentUser me, IAiProvider ai, IOptions<AiOptions> aiOptions)
    {
        _db = db;
        _me = me;
        _ai = ai;
        _aiOptions = aiOptions.Value;
    }

    /// <summary>The user's conversation (created on first access) + AI availability.</summary>
    [HttpGet]
    public async Task<ActionResult<CoachStateDto>> Get(CancellationToken ct)
    {
        var conversation = await GetOrCreateConversationAsync(ct);
        return Ok(ToState(conversation));
    }

    /// <summary>Clear the conversation and start fresh.</summary>
    [HttpDelete]
    public async Task<IActionResult> Reset(CancellationToken ct)
    {
        var conversation = await _db.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.UserId == _me.Id, ct);
        if (conversation is not null)
        {
            _db.ChatMessages.RemoveRange(conversation.Messages);
            conversation.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
        return NoContent();
    }

    /// <summary>
    /// Post a user message and stream the assistant reply as Server-Sent Events.
    /// Both messages are persisted so the conversation survives across sessions.
    /// </summary>
    [HttpPost("messages")]
    public async Task Send(SendMessageRequest request, CancellationToken ct)
    {
        var content = request.Content?.Trim() ?? string.Empty;
        if (content.Length == 0)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsJsonAsync(new { message = "Message can't be empty." }, ct);
            return;
        }

        var conversation = await GetOrCreateConversationAsync(ct);
        conversation.Messages.Add(new ChatMessage
        {
            Role = ChatRole.User,
            Content = content,
            CreatedAt = DateTimeOffset.UtcNow,
        });
        conversation.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        var history = conversation.Messages
            .OrderBy(m => m.Id)
            .Select(m => new AiTurn(m.Role == ChatRole.User ? "user" : "assistant", m.Content))
            .ToList();

        // Start SSE.
        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";
        HttpContext.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();

        var systemPrompt = SystemPrompt + await BuildContextAsync(ct);

        var assistant = new System.Text.StringBuilder();
        try
        {
            await foreach (var chunk in _ai.StreamReplyAsync(systemPrompt, history, ct))
            {
                assistant.Append(chunk);
                await WriteEventAsync(new { type = "delta", text = chunk }, ct);
            }
        }
        catch (OperationCanceledException)
        {
            // Client went away — nothing to report.
        }
        catch (Exception)
        {
            await WriteEventAsync(new { type = "error", message = "The coach is unavailable right now. Please try again." }, ct);
        }

        // Persist whatever the assistant produced.
        if (assistant.Length > 0)
        {
            conversation.Messages.Add(new ChatMessage
            {
                Role = ChatRole.Assistant,
                Content = assistant.ToString(),
                CreatedAt = DateTimeOffset.UtcNow,
            });
            conversation.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        await WriteEventAsync(new { type = "done" }, ct);
    }

    /// <summary>
    /// Builds a dynamic context block (equipment, scheduled program, nutrition
    /// targets) appended to the system prompt so the coach's advice fits what the
    /// user actually owns and is doing. Best-effort — missing data is simply omitted.
    /// </summary>
    private async Task<string> BuildContextAsync(CancellationToken ct)
    {
        var sb = new System.Text.StringBuilder();

        var equipment = await _db.EquipmentItems.AsNoTracking()
            .Where(e => e.UserId == _me.Id)
            .OrderBy(e => e.SortOrder)
            .Select(e => new { e.Name, e.Specs, e.Status })
            .ToListAsync(ct);
        if (equipment.Count > 0)
        {
            var owned = equipment.Where(e => e.Status == EquipmentStatus.Owned)
                .Select(e => e.Specs is null ? e.Name : $"{e.Name} ({e.Specs})");
            sb.Append("\n\nEquipment the user owns: ").Append(string.Join("; ", owned)).Append('.');
            var planned = equipment.Where(e => e.Status == EquipmentStatus.Planned).Select(e => e.Name).ToList();
            if (planned.Count > 0)
            {
                sb.Append(" Planning to get: ").Append(string.Join("; ", planned)).Append('.');
            }
        }

        var days = await _db.WorkoutDays.AsNoTracking()
            .Where(d => d.UserId == _me.Id)
            .OrderBy(d => d.SortOrder)
            .Select(d => new { d.Name, d.ScheduledDayOfWeek })
            .ToListAsync(ct);
        if (days.Count > 0)
        {
            var scheduled = days.Select(d => d.ScheduledDayOfWeek is null
                ? d.Name
                : $"{d.ScheduledDayOfWeek} — {d.Name}");
            sb.Append("\n\nCurrent weekly program: ").Append(string.Join("; ", scheduled)).Append('.');
        }

        var profile = await _db.NutritionProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == _me.Id, ct);
        if (profile is not null)
        {
            sb.Append("\n\nNutrition: ").Append(profile.DietStyle)
                .Append($", bodyweight {profile.WeightKg:0.#}kg, protein target {profile.ProteinTargetG}g/day");
            if (!string.IsNullOrWhiteSpace(profile.Notes))
            {
                sb.Append(". ").Append(profile.Notes);
            }
            sb.Append('.');
        }

        return sb.ToString();
    }

    private async Task WriteEventAsync(object payload, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(payload);
        await Response.WriteAsync($"data: {json}\n\n", ct);
        await Response.Body.FlushAsync(ct);
    }

    private async Task<Conversation> GetOrCreateConversationAsync(CancellationToken ct)
    {
        var conversation = await _db.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.UserId == _me.Id, ct);
        if (conversation is null)
        {
            conversation = new Conversation
            {
                UserId = _me.Id,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };
            _db.Conversations.Add(conversation);
            await _db.SaveChangesAsync(ct);
        }
        return conversation;
    }

    private CoachStateDto ToState(Conversation conversation) => new(
        _aiOptions.Enabled,
        _ai.Name,
        conversation.Messages
            .OrderBy(m => m.Id)
            .Select(m => new CoachMessageDto(
                m.Id,
                m.Role == ChatRole.User ? "user" : "assistant",
                m.Content,
                m.CreatedAt))
            .ToList());
}
