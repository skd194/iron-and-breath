using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Auth;

/// <summary>
/// Gives a brand-new user their own editable program by cloning the seeded
/// template days/exercises (UserId == null) into user-owned rows, and creates
/// their settings row. Shared videos are referenced, not copied.
/// </summary>
public class UserProvisioningService
{
    private readonly AppDbContext _db;

    public UserProvisioningService(AppDbContext db) => _db = db;

    /// <summary>Requires <paramref name="user"/> to already be persisted (has an Id).</summary>
    public async Task ProvisionAsync(User user, CancellationToken ct = default)
    {
        var templates = await _db.WorkoutDays
            .AsNoTracking()
            .Where(d => d.UserId == null)
            .Include(d => d.Exercises)
            .OrderBy(d => d.SortOrder)
            .ToListAsync(ct);

        foreach (var template in templates)
        {
            var day = new WorkoutDay
            {
                UserId = user.Id,
                Name = template.Name,
                Focus = template.Focus,
                SortOrder = template.SortOrder,
            };

            foreach (var ex in template.Exercises.OrderBy(e => e.SortOrder))
            {
                day.Exercises.Add(new Exercise
                {
                    Name = ex.Name,
                    TargetRepsLow = ex.TargetRepsLow,
                    TargetRepsHigh = ex.TargetRepsHigh,
                    RepsDisplay = ex.RepsDisplay,
                    Cue = ex.Cue,
                    SortOrder = ex.SortOrder,
                    BaseSets = ex.BaseSets,
                    VideoId = ex.VideoId,
                    // Coaching metadata carries into the user's editable copy.
                    BreathingConcentric = ex.BreathingConcentric,
                    BreathingEccentric = ex.BreathingEccentric,
                    BreathingNotes = ex.BreathingNotes,
                    PrimaryMuscles = ex.PrimaryMuscles,
                    SecondaryMuscles = ex.SecondaryMuscles,
                    Tempo = ex.Tempo,
                    Benefits = ex.Benefits,
                    CommonMistakes = ex.CommonMistakes,
                    SafetyTips = ex.SafetyTips,
                    AnimationRef = ex.AnimationRef,
                });
            }

            _db.WorkoutDays.Add(day);
        }

        _db.UserProgramSettings.Add(new UserProgramSettings
        {
            UserId = user.Id,
            ProgramStartDate = DateOnly.FromDateTime(DateTime.Today),
            DaysPerWeekTarget = 4,
        });

        await _db.SaveChangesAsync(ct);
    }
}
