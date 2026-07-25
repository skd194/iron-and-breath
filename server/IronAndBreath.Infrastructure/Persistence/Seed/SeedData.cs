using IronAndBreath.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Infrastructure.Persistence.Seed;

/// <summary>
/// Static reference data baked into migrations via HasData. This covers the
/// 4-day split, its exercises, and the three progression phases. Videos are
/// seeded separately (milestone 5) and the settings row is created at runtime.
/// </summary>
public static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkoutDay>().HasData(
            new WorkoutDay { Id = 1, Name = "Upper Body (Push)", Focus = "Chest, shoulders, triceps", SortOrder = 1 },
            new WorkoutDay { Id = 2, Name = "Lower Body + Core", Focus = "Quads, hamstrings, glutes, core", SortOrder = 2 },
            new WorkoutDay { Id = 3, Name = "Upper Body (Pull)", Focus = "Back, biceps, rear delts", SortOrder = 3 },
            new WorkoutDay { Id = 4, Name = "Full Body (Metabolic)", Focus = "Total body, conditioning, core", SortOrder = 4 }
        );

        modelBuilder.Entity<Exercise>().HasData(
            // Day 1 — Upper Body (Push)
            Ex(1, 1, "Dumbbell Floor Press", 10, 12, "4 sets x 10-12 reps", 4, 1, "Lower elbows to the floor, pause, press up."),
            Ex(2, 1, "Dumbbell Shoulder Press", 10, 12, "3 sets x 10-12 reps", 3, 2, "Brace core, press overhead without arching the back."),
            Ex(3, 1, "Dumbbell Floor Flyes", 12, 15, "3 sets x 12-15 reps", 3, 3, "Soft elbows, wide arc, squeeze the chest at the top."),
            Ex(4, 1, "Push-ups", null, null, "To near-failure", 3, 4, "Straight line head-to-heels, full range of motion."),
            Ex(5, 1, "Lateral Raises", 15, 15, "3 sets x 15 reps", 3, 5, "Lead with the elbows, raise to shoulder height only."),
            Ex(6, 1, "Tricep Dumbbell Extension", 12, 15, "3 sets x 12-15 reps", 3, 6, "Keep elbows tucked, control the negative."),

            // Day 2 — Lower Body + Core
            Ex(7, 2, "Goblet Squat", 12, 15, "4 sets x 12-15 reps", 4, 1, "Chest up, sit between the hips, knees track toes."),
            Ex(8, 2, "Dumbbell Romanian Deadlift", 12, 12, "4 sets x 12 reps", 4, 2, "Hinge at the hips, soft knees, feel the hamstrings."),
            Ex(9, 2, "Walking Lunges", 12, 12, "3 sets x 12 reps/leg", 3, 3, "Long stride, back knee toward the floor, torso tall."),
            Ex(10, 2, "Standing Calf Raise", 20, 20, "3 sets x 20 reps", 3, 4, "Full stretch at the bottom, pause at the top."),
            Ex(11, 2, "Plank Hold", null, null, "45-60 sec", 3, 5, "Squeeze glutes and abs, no sagging hips."),
            Ex(12, 2, "Lying Leg Raises", 15, 15, "3 sets x 15 reps", 3, 6, "Press low back down, lower legs slowly."),

            // Day 3 — Upper Body (Pull)
            Ex(13, 3, "Single-Arm Dumbbell Row", 10, 12, "4 sets x 10-12 reps/side", 4, 1, "Flat back, drive elbow to hip, squeeze the lat."),
            Ex(14, 3, "Renegade Row", 10, 10, "3 sets x 10 reps/side", 3, 2, "Wide feet for stability, minimize hip rotation."),
            Ex(15, 3, "Bicep Curl", 12, 15, "3 sets x 12-15 reps", 3, 3, "Elbows pinned, no swinging, full squeeze."),
            Ex(16, 3, "Hammer Curl", 12, 15, "3 sets x 12-15 reps", 3, 4, "Neutral grip, control both directions."),
            Ex(17, 3, "Rear Delt Flye", 15, 15, "3 sets x 15 reps", 3, 5, "Hinge forward, raise wide, lead with pinkies."),
            Ex(18, 3, "Towel Row Under Table (or Pull-ups)", null, null, "To near-failure", 3, 6, "Drive elbows down and back, chest to the edge."),

            // Day 4 — Full Body (Metabolic)
            Ex(19, 4, "Dumbbell Thruster", 10, 12, "4 sets x 10-12 reps", 4, 1, "Squat, then drive through into the press in one motion."),
            Ex(20, 4, "Dumbbell Deadlift", 12, 12, "3 sets x 12 reps", 3, 2, "Neutral spine, push the floor away, lock out tall."),
            Ex(21, 4, "Push-ups", null, null, "To near-failure", 3, 3, "Straight line head-to-heels, full range of motion."),
            Ex(22, 4, "Bent-Over Row", 12, 12, "3 sets x 12 reps", 3, 4, "Hinge to ~45 degrees, row to the lower ribs."),
            Ex(23, 4, "Russian Twist", 20, 20, "3 sets x 20 reps", 3, 5, "Rotate from the trunk, touch each side."),
            Ex(24, 4, "Bicycle Crunch", 20, 20, "3 sets x 20 reps", 3, 6, "Opposite elbow to knee, slow and controlled.")
        );

        modelBuilder.Entity<ProgramProgressionPhase>().HasData(
            new ProgramProgressionPhase
            {
                Id = 1, PhaseNumber = 1, WeekStart = 1, WeekEnd = 4,
                RepsHintText = "10-12 reps, slow controlled tempo",
                SetDelta = 0, RestWorkSeconds = 40, RestBetweenSetsSeconds = 90, RestBetweenExercisesSeconds = 120,
                Notes = "Focus on slow, controlled tempo and full range of motion."
            },
            new ProgramProgressionPhase
            {
                Id = 2, PhaseNumber = 2, WeekStart = 5, WeekEnd = 8,
                RepsHintText = "15-20 reps, higher volume",
                SetDelta = 0, RestWorkSeconds = 40, RestBetweenSetsSeconds = 60, RestBetweenExercisesSeconds = 90,
                Notes = "Shorter rest. Encourage supersetting where possible."
            },
            new ProgramProgressionPhase
            {
                Id = 3, PhaseNumber = 3, WeekStart = 9, WeekEnd = 12,
                RepsHintText = "Add a set, pause reps, shortest rest",
                SetDelta = 1, RestWorkSeconds = 45, RestBetweenSetsSeconds = 45, RestBetweenExercisesSeconds = 75,
                Notes = "Pause reps for tension. Optional drop set on the last set of each exercise."
            }
        );
    }

    private static Exercise Ex(int id, int dayId, string name, int? low, int? high, string display, int baseSets, int sortOrder, string cue)
        => new()
        {
            Id = id,
            WorkoutDayId = dayId,
            Name = name,
            TargetRepsLow = low,
            TargetRepsHigh = high,
            RepsDisplay = display,
            BaseSets = baseSets,
            SortOrder = sortOrder,
            Cue = cue,
            VideoId = null
        };
}
