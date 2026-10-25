using IronAndBreath.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Infrastructure.Persistence.Seed;

/// <summary>
/// Static reference data baked into migrations via HasData. Covers the 5-day
/// muscle-building split (scheduled Sat/Sun/Mon/Wed/Thu), its exercises with full
/// coaching + tempo metadata, the three progression phases, the muscle reference
/// catalog, the equipment template, and the weekly vegetarian meal-plan template.
/// Per-user rows (UserId == null templates) are cloned at provisioning; videos and
/// the settings row are handled elsewhere.
/// </summary>
public static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkoutDay>().HasData(
            Day(1, "Push · Chest / Shoulders / Triceps", "Horizontal + overhead pressing", 1, DayOfWeek.Saturday),
            Day(2, "Pull · Back / Biceps / Rear Delts", "Rows, pulls, curls, back health", 2, DayOfWeek.Sunday),
            Day(3, "Legs + Core", "Squat, hinge, lunge, trunk", 3, DayOfWeek.Monday),
            Day(4, "Full-Body Metabolic + Cardio", "Compound circuit with skipping", 4, DayOfWeek.Wednesday),
            Day(5, "Mobility & Yoga", "Full-body stretch, back-focused recovery", 5, DayOfWeek.Thursday)
        );

        modelBuilder.Entity<Exercise>().HasData(
            // ---- Day 1 — Push (Saturday) ----
            Ex(1, 1, 1, "Flat Barbell Bench Press", 8, 12, "4 sets x 8-12 reps", 4,
                "Chest,Triceps", "Shoulders",
                "Shoulder blades pinched, bar to mid-chest, drive up and slightly back.",
                benefits: "The foundational horizontal press for chest and triceps mass.",
                mistakes: "Flaring elbows to 90°, bouncing the bar off the chest.",
                safety: "Keep wrists stacked over elbows; use a spotter or safety at light-but-heavy loads.",
                con: 2, ecc: 3),
            Ex(2, 1, 2, "Incline Dumbbell Press", 10, 12, "3 sets x 10-12 reps", 3,
                "Upper Chest,Shoulders", "Triceps",
                "Bench at 30°, press the dumbbells up and together without clanging.",
                benefits: "Targets the upper chest the flat press under-trains.",
                mistakes: "Setting the bench too steep (turns it into a shoulder press).",
                safety: "Control the dumbbells at the bottom; don't let shoulders roll forward.",
                con: 2, ecc: 3),
            Ex(3, 1, 3, "Incline Dumbbell Flye", 12, 15, "3 sets x 12-15 reps", 3,
                "Chest", "Shoulders",
                "Soft elbows, wide arc, feel the stretch, squeeze at the top.",
                benefits: "Isolates the chest through a long stretch for extra hypertrophy.",
                mistakes: "Bending the elbows into a press, going too heavy.",
                safety: "Keep a slight elbow bend throughout to protect the shoulder joint.",
                con: 2, ecc: 3),
            Ex(4, 1, 4, "Seated Dumbbell Shoulder Press", 10, 12, "3 sets x 10-12 reps", 3,
                "Shoulders,Triceps", "Upper Chest",
                "Brace the core, press overhead without arching the lower back.",
                benefits: "Builds overhead pressing strength and delt size.",
                mistakes: "Over-arching the back, pressing with a forward lean.",
                safety: "Keep ribs down; stop the range where shoulders feel pinch-free.",
                con: 2, ecc: 3),
            Ex(5, 1, 5, "Lateral Raise", 15, 20, "3 sets x 15-20 reps", 3,
                "Shoulders", "Traps",
                "Lead with the elbows, raise to shoulder height only, control down.",
                benefits: "Widens the shoulders by isolating the side delts.",
                mistakes: "Swinging the weight, shrugging the traps up.",
                safety: "Go light; momentum here strains the shoulder and neck.",
                con: 2, ecc: 3),
            Ex(6, 1, 6, "Overhead Triceps Extension", 12, 15, "3 sets x 12-15 reps", 3,
                "Triceps", "Shoulders",
                "Elbows tucked and pointing forward, full stretch behind the head.",
                benefits: "Hits the long head of the triceps through a deep stretch.",
                mistakes: "Flaring the elbows out, using the shoulders to lift.",
                safety: "Lower under control; don't let the weight yank the elbow open.",
                con: 2, ecc: 3),
            Ex(7, 1, 7, "Push-ups", null, null, "2 sets to near-failure", 2,
                "Chest,Triceps", "Shoulders,Core",
                "Straight line head-to-heels, full range, chest to the floor.",
                benefits: "A bodyweight finisher to fully fatigue the chest and triceps.",
                mistakes: "Sagging hips, half-range reps.",
                safety: "Keep the core braced to protect the lower back.",
                con: 2, ecc: 3),

            // ---- Day 2 — Pull (Sunday) ----
            Ex(8, 2, 1, "Barbell Bent-Over Row", 8, 12, "4 sets x 8-12 reps", 4,
                "Back,Lats", "Biceps,Rear Delts",
                "Hinge to ~45°, flat back, pull the bar to the lower ribs.",
                benefits: "The primary mass builder for the whole back.",
                mistakes: "Rounding the back, jerking with the hips.",
                safety: "Keep a neutral spine; brace hard to protect the lower back.",
                con: 2, ecc: 3),
            Ex(9, 2, 2, "One-Arm Dumbbell Row", 10, 12, "3 sets x 10-12 reps/side", 3,
                "Lats,Back", "Biceps",
                "Hand and knee on the bench, drive the elbow to the hip, squeeze the lat.",
                benefits: "Unilateral row that fixes left/right imbalances.",
                mistakes: "Twisting the torso to heave the weight up.",
                safety: "Keep the spine neutral and hips square.",
                con: 2, ecc: 3),
            Ex(10, 2, 3, "Pull-ups", 6, 10, "3 sets x 6-10 reps (or to near-failure)", 3,
                "Back,Lats", "Biceps,Rear Delts",
                "Dead hang from the doorway bar, pull your chin over, lower under control.",
                benefits: "The best bodyweight back and lat builder — uses your pull-up bar.",
                mistakes: "Kipping/swinging, half reps, not controlling the descent.",
                safety: "Full controlled range; use a band or slow negatives if you can't do full reps yet.",
                con: 2, ecc: 3),
            Ex(11, 2, 4, "Barbell Curl", 10, 12, "3 sets x 10-12 reps", 3,
                "Biceps", "Forearms",
                "Elbows pinned to the sides, curl the curl-rod, no swinging.",
                benefits: "The classic mass builder for the biceps.",
                mistakes: "Swinging the torso, using the shoulders to lift.",
                safety: "Keep wrists neutral; the EZ bar is easier on them.",
                con: 2, ecc: 3),
            Ex(12, 2, 5, "Hammer Curl", 12, 15, "3 sets x 12-15 reps", 3,
                "Biceps,Forearms", null,
                "Neutral grip (palms facing in), control both directions.",
                benefits: "Builds the brachialis for thicker arms and strong forearms.",
                mistakes: "Rushing the lowering phase.",
                safety: "No swinging; keep the elbows still.",
                con: 2, ecc: 3),
            Ex(13, 2, 6, "Rear-Delt Flye", 15, 20, "3 sets x 15-20 reps", 3,
                "Rear Delts", "Traps,Back",
                "Hinge forward, raise the dumbbells wide, lead with the pinkies.",
                benefits: "Balances the shoulders and improves posture.",
                mistakes: "Using the lower back to swing the weight.",
                safety: "Light weight, slow tempo; protect the lower back by bracing.",
                con: 2, ecc: 3),
            Ex(14, 2, 7, "Superman / Back Extension Hold", null, null, "3 sets x 30-45 sec", 3,
                "Lower Back,Glutes", "Core",
                "Lift chest and thighs off the floor, squeeze the glutes, breathe.",
                benefits: "Strengthens the spinal erectors to relieve back tightness.",
                mistakes: "Cranking the neck back.",
                safety: "Keep the neck neutral; stop if it pinches the lower back.",
                hold: true, holdBreath: "Breathe slow and steady — don't hold your breath."),

            // ---- Day 3 — Legs + Core (Monday) ----
            Ex(15, 3, 1, "Barbell Back Squat", 8, 12, "4 sets x 8-12 reps", 4,
                "Quadriceps,Glutes", "Hamstrings,Core",
                "Chest up, sit between the hips, knees track the toes, drive up.",
                benefits: "The king of lower-body mass and strength.",
                mistakes: "Knees caving in, heels lifting, rounding the back.",
                safety: "Brace the core; keep depth where your back stays neutral.",
                con: 2, ecc: 3),
            Ex(16, 3, 2, "Romanian Deadlift", 10, 12, "4 sets x 10-12 reps", 4,
                "Hamstrings,Glutes", "Lower Back",
                "Hinge at the hips, soft knees, bar close, feel the hamstring stretch.",
                benefits: "Builds the hamstrings and glutes and protects the back.",
                mistakes: "Rounding the back, squatting instead of hinging.",
                safety: "Keep the bar against the legs and the spine neutral throughout.",
                con: 2, ecc: 3),
            Ex(17, 3, 3, "Bulgarian Split Squat", 10, 12, "3 sets x 10-12 reps/leg", 3,
                "Quadriceps,Glutes", "Hamstrings",
                "Rear foot on the bench, drop straight down, drive through the front heel.",
                benefits: "Unilateral leg builder that also trains balance.",
                mistakes: "Leaning too far forward, letting the front knee cave.",
                safety: "Keep the front knee tracking over the foot.",
                con: 2, ecc: 3),
            Ex(18, 3, 4, "Walking Lunge", 12, 12, "3 sets x 12 reps/leg", 3,
                "Quadriceps,Glutes", "Hamstrings,Calves",
                "Long stride, back knee toward the floor, torso tall.",
                benefits: "Trains the legs through a long, athletic range.",
                mistakes: "Short steps that overload the knee.",
                safety: "Step long enough that the front shin stays near vertical.",
                con: 2, ecc: 3),
            Ex(19, 3, 5, "Standing Calf Raise", 20, 20, "4 sets x 20 reps", 4,
                "Calves", null,
                "Full stretch at the bottom, pause and squeeze at the top.",
                benefits: "Builds the calves, which need high reps and a strong stretch.",
                mistakes: "Bouncing through a short range.",
                safety: "Control the bottom stretch; don't ballistically drop.",
                con: 2, ecc: 3, pause: 1),
            Ex(20, 3, 6, "Plank Hold", null, null, "3 sets x 45-60 sec", 3,
                "Core", "Shoulders,Glutes",
                "Squeeze the glutes and abs, straight line, no sagging hips.",
                benefits: "Builds deep core stability that protects the spine.",
                mistakes: "Hips sagging or piking up.",
                safety: "Keep a neutral neck and stop before the lower back sags.",
                hold: true, holdBreath: "Breathe slow and steady — don't hold your breath."),
            Ex(21, 3, 7, "Hanging Leg Raise", 10, 15, "3 sets x 10-15 reps", 3,
                "Core", "Hip Flexors,Forearms",
                "Hang from the doorway bar, raise the legs with control, don't swing.",
                benefits: "A strong lower-ab and grip builder using your pull-up bar.",
                mistakes: "Swinging the legs up with momentum.",
                safety: "Keep it controlled; stop if the lower back rounds painfully.",
                con: 2, ecc: 3),

            // ---- Day 4 — Full-Body Metabolic + Cardio (Wednesday) ----
            Ex(22, 4, 1, "Skipping — Warm-up Intervals", null, null, "3-5 min easy pace", 1,
                "Calves,Shoulders", "Core",
                "Light bounces on the balls of the feet, relaxed shoulders.",
                benefits: "Raises the heart rate and primes the whole body.",
                mistakes: "Jumping too high and landing hard.",
                safety: "Land softly through the balls of the feet to spare the knees.",
                hold: true, holdBreath: "Breathe rhythmically with the rope."),
            Ex(23, 4, 2, "Dumbbell Thruster", 10, 12, "4 sets x 10-12 reps", 4,
                "Quadriceps,Shoulders", "Glutes,Triceps,Core",
                "Squat, then drive up and press overhead in one fluid motion.",
                benefits: "A full-body compound that burns a lot in little time.",
                mistakes: "Pressing before the legs finish driving.",
                safety: "Keep the core braced; don't arch the back at lockout.",
                con: 2, ecc: 3),
            Ex(24, 4, 3, "Dumbbell Deadlift", 12, 12, "3 sets x 12 reps", 3,
                "Hamstrings,Glutes", "Back,Core",
                "Neutral spine, push the floor away, lock out tall.",
                benefits: "Trains the posterior chain safely with lighter load.",
                mistakes: "Rounding the back, jerking off the floor.",
                safety: "Hinge, don't squat; keep the spine neutral.",
                con: 2, ecc: 3),
            Ex(25, 4, 4, "Renegade Row", 10, 10, "3 sets x 10 reps/side", 3,
                "Back,Core", "Biceps,Shoulders",
                "Wide feet for stability, row one dumbbell, minimise hip rotation.",
                benefits: "Combines a row with a hard anti-rotation core challenge.",
                mistakes: "Twisting the hips toward the working arm.",
                safety: "Set feet wide and keep the hips level.",
                con: 2, ecc: 3),
            Ex(26, 4, 5, "Push-ups", null, null, "3 sets to near-failure", 3,
                "Chest,Triceps", "Shoulders,Core",
                "Straight line head-to-heels, full range of motion.",
                benefits: "Bodyweight pressing volume for conditioning.",
                mistakes: "Sagging hips, partial reps.",
                safety: "Brace the core to protect the lower back.",
                con: 2, ecc: 3),
            Ex(27, 4, 6, "Russian Twist", 20, 20, "3 sets x 20 reps", 3,
                "Core,Obliques", "Hip Flexors",
                "Rotate from the trunk, touch each side, feet hovering if able.",
                benefits: "Builds rotational core strength and the obliques.",
                mistakes: "Moving only the arms, not the torso.",
                safety: "Rotate from the ribcage, not by yanking the lower back.",
                con: 1, ecc: 1),
            Ex(28, 4, 7, "Skipping — Finisher", null, null, "2-3 min, build pace", 1,
                "Calves,Shoulders", "Core",
                "Steady rhythm, pick up the pace for the last 30 seconds.",
                benefits: "Caps the session with a conditioning burst.",
                mistakes: "Tensing the shoulders.",
                safety: "Keep landings soft; stop if the calves cramp.",
                hold: true, holdBreath: "Breathe rhythmically with the rope."),

            // ---- Day 5 — Mobility & Yoga (Thursday) — all gentle holds ----
            Ex(29, 5, 1, "Cat-Cow", null, null, "60 sec, slow flow", 1,
                "Spinal Erectors,Core", "Lower Back",
                "Alternate arching and rounding the spine with the breath.",
                benefits: "Mobilises the whole spine and eases back stiffness.",
                mistakes: "Rushing; forcing the range.",
                safety: "Move only as far as comfortable.",
                hold: true, holdBreath: "Inhale to arch, exhale to round."),
            Ex(30, 5, 2, "Child's Pose", null, null, "60 sec hold", 1,
                "Lats,Lower Back", "Shoulders",
                "Hips back to the heels, arms long, let the back decompress.",
                benefits: "Gently stretches the back and lats and calms the nervous system.",
                mistakes: "Holding tension in the shoulders.",
                safety: "Widen the knees if the belly or hips feel cramped.",
                hold: true, holdBreath: "Breathe deep into the back of the ribs."),
            Ex(31, 5, 3, "Downward Dog", null, null, "45 sec hold", 1,
                "Hamstrings,Calves", "Shoulders,Back",
                "Hips high, heels reaching down, spine long.",
                benefits: "Stretches the whole back line — calves, hamstrings, spine.",
                mistakes: "Rounding the back to force the heels down.",
                safety: "Bend the knees as needed to keep the spine long.",
                hold: true, holdBreath: "Breathe slow and steady."),
            Ex(32, 5, 4, "Cobra / Sphinx", null, null, "45 sec hold", 1,
                "Lower Back,Core", "Chest",
                "Lift the chest, elbows under shoulders, shoulders down away from ears.",
                benefits: "Counters hunching and gently extends the lower back.",
                mistakes: "Cranking the neck or over-extending.",
                safety: "Keep the lift gentle; no sharp lower-back pinch.",
                hold: true, holdBreath: "Breathe into the chest."),
            Ex(33, 5, 5, "Thread-the-Needle", null, null, "45 sec/side", 1,
                "Upper Back,Rear Delts", "Shoulders",
                "From all fours, thread one arm under the other, rest on the shoulder.",
                benefits: "Opens the upper back and shoulders.",
                mistakes: "Collapsing the supporting shoulder.",
                safety: "Keep weight evenly distributed; move slowly.",
                hold: true, holdBreath: "Breathe slow and steady."),
            Ex(34, 5, 6, "Supine Spinal Twist", null, null, "45 sec/side", 1,
                "Lower Back,Obliques", "Glutes,Spinal Erectors",
                "On your back, drop the knees to one side, arms wide, gaze opposite.",
                benefits: "Releases the lower back and obliques — great for tightness.",
                mistakes: "Forcing the knees down and lifting the shoulder.",
                safety: "Keep both shoulders on the floor; let gravity do the work.",
                hold: true, holdBreath: "Exhale as you sink deeper."),
            Ex(35, 5, 7, "Seated Forward Fold", null, null, "60 sec hold", 1,
                "Hamstrings,Lower Back", "Calves",
                "Hinge from the hips, long spine, reach toward the feet.",
                benefits: "Lengthens the hamstrings and the whole back line.",
                mistakes: "Rounding hard from the upper back to reach further.",
                safety: "Bend the knees slightly; lead with the chest, not the head.",
                hold: true, holdBreath: "Exhale to fold a little deeper."),
            Ex(36, 5, 8, "Pigeon Pose", null, null, "45 sec/side", 1,
                "Glutes,Hip Flexors", "Lower Back",
                "Front shin angled, back leg long, fold over the front leg.",
                benefits: "Opens tight hips and glutes that pull on the lower back.",
                mistakes: "Collapsing onto one hip.",
                safety: "Prop a cushion under the hip to keep it level.",
                hold: true, holdBreath: "Breathe slow and steady."),
            Ex(37, 5, 9, "Standing Hamstring + Hip-Flexor Stretch", null, null, "45 sec/side", 1,
                "Hamstrings,Hip Flexors", "Quadriceps",
                "Stretch one hamstring, then lunge to open the opposite hip flexor.",
                benefits: "Balances the front and back of the hips for a neutral pelvis.",
                mistakes: "Bouncing into the stretch.",
                safety: "Ease in gradually; never bounce.",
                hold: true, holdBreath: "Breathe slow and steady."),
            Ex(38, 5, 10, "Neck & Shoulder Release", null, null, "45 sec hold", 1,
                "Traps,Neck", "Shoulders",
                "Gently tilt the ear to the shoulder, roll the shoulders back and down.",
                benefits: "Releases desk-and-training tension in the neck and traps.",
                mistakes: "Pulling the head hard with the hand.",
                safety: "Use only light pressure; the neck is delicate.",
                hold: true, holdBreath: "Breathe slow and steady.")
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

        SeedMuscles(modelBuilder);
        SeedEquipment(modelBuilder);
        SeedDiet(modelBuilder);
    }

    // ---- Muscle reference catalog ----

    private static void SeedMuscles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Muscle>().HasData(
            Mu(1, "Chest", "Chest", "The pectorals push things away from you and across your body — pressing, pushing doors, throwing.", "Warm up the shoulders; avoid over-flaring the elbows when pressing."),
            Mu(2, "Upper Chest", "Chest", "The clavicular head of the chest drives pressing on an incline and from low to high.", "Don't set incline benches too steep or the shoulders take over."),
            Mu(3, "Triceps", "Arms", "The three-headed muscle on the back of the arm straightens the elbow — every push and lockout.", "Control overhead extensions; the long head is easy to strain cold."),
            Mu(4, "Shoulders", "Shoulders", "The deltoids raise and press the arm overhead and out to the sides — everything overhead.", "Press pain-free ranges only; the shoulder is the body's most mobile, least stable joint."),
            Mu(5, "Rear Delts", "Shoulders", "The rear deltoids pull the arm back and support posture — the antidote to hunching.", "Train light and controlled; heavy swinging strains the shoulder."),
            Mu(6, "Traps", "Back", "The trapezius shrugs, rotates and stabilises the shoulder blades and supports the neck.", "Avoid over-shrugging during raises; keep the neck long."),
            Mu(7, "Back", "Back", "The mid-back muscles pull, row and hold you upright with good posture.", "Keep a neutral spine when rowing; never round under load."),
            Mu(8, "Upper Back", "Back", "The rhomboids and mid-traps retract the shoulder blades and keep the chest open.", "Strengthen to counter desk posture; move slowly in stretches."),
            Mu(9, "Lats", "Back", "The latissimus dorsi pulls the arms down and back — pull-ups, rows, the V-taper.", "Keep the shoulder down and back; don't let it roll forward at the stretch."),
            Mu(10, "Biceps", "Arms", "The biceps bend the elbow and turn the palm up — curling and pulling.", "Avoid swinging; jerking loads strains the elbow tendon."),
            Mu(11, "Forearms", "Arms", "The forearm muscles grip, and bend and rotate the wrist — grip strength and carrying.", "Build grip gradually; wrist tendons flare with sudden volume."),
            Mu(12, "Lower Back", "Core", "The spinal erectors hold you upright and extend the spine — hinging, lifting, standing tall.", "Brace and hinge correctly; most back pain comes from rounding under load."),
            Mu(13, "Spinal Erectors", "Core", "The long muscles either side of the spine extend and stabilise the back through every lift.", "Keep a neutral spine; strengthen them to relieve tightness."),
            Mu(14, "Glutes", "Legs", "The glutes extend the hip and drive you forward — standing, squatting, sprinting, jumping.", "Activate them so the lower back doesn't take over hinging."),
            Mu(15, "Core", "Core", "The abdominals brace the trunk and resist bending and twisting — they protect the spine in every lift.", "Train bracing, not just crunching; a strong core spares the back."),
            Mu(16, "Obliques", "Core", "The side-abdominals rotate and side-bend the trunk and resist unwanted twisting.", "Rotate from the ribcage, not the lower back."),
            Mu(17, "Quadriceps", "Legs", "The front-thigh muscles straighten the knee — squatting, standing, climbing stairs.", "Keep the knee tracking over the foot; don't let it cave in."),
            Mu(18, "Hamstrings", "Legs", "The back-thigh muscles bend the knee and extend the hip — hinging, running, bracing the knee.", "Stretch and strengthen; tight hamstrings tug on the lower back."),
            Mu(19, "Calves", "Legs", "The calves point the foot and drive push-off — walking, running, jumping.", "Train a full stretch; respond best to high reps and a pause."),
            Mu(20, "Hip Flexors", "Legs", "The hip flexors lift the thigh toward the torso — walking, running, raising the legs.", "Stretch them regularly; sitting keeps them tight and tilts the pelvis."),
            Mu(21, "Neck", "Neck", "The neck muscles support and move the head and anchor the upper traps.", "Use only gentle pressure when stretching; the neck is delicate.")
        );
    }

    // ---- Equipment template (cloned into each user's inventory) ----

    private static void SeedEquipment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EquipmentItem>().HasData(
            Eq(1, "Adjustable Bench", EquipmentCategory.Bench, "Incline / decline / flat, 250 kg capacity, resistance ropes included.", EquipmentStatus.Owned, 1),
            Eq(2, "Straight Barbell (5 ft, 25 mm)", EquipmentCategory.Barbell, "For bench press, squats, rows and deadlifts.", EquipmentStatus.Owned, 2),
            Eq(3, "EZ Curl Rod (3 ft)", EquipmentCategory.Barbell, "For curls and overhead triceps extensions.", EquipmentStatus.Owned, 3),
            Eq(4, "Dumbbell Handles (pair, 14 in)", EquipmentCategory.Dumbbell, "Loadable dumbbell rods with collars.", EquipmentStatus.Owned, 4),
            Eq(5, "Weight Plates", EquipmentCategory.Plates, "4 × 2.5 kg + 4 × 2 kg rubber plates (18 kg total).", EquipmentStatus.Owned, 5),
            Eq(6, "Doorway Pull-up Bar", EquipmentCategory.Bodyweight, "Fits in a doorway — pull-ups, chin-ups and hanging leg raises.", EquipmentStatus.Owned, 6),
            Eq(7, "Skipping Rope", EquipmentCategory.Cardio, "Warm-ups and conditioning intervals.", EquipmentStatus.Owned, 7),
            Eq(8, "Heavier Plates", EquipmentCategory.Plates, "Add 5 kg / 10 kg plates as the lifts get strong.", EquipmentStatus.Planned, 8)
        );
    }

    // ---- Weekly vegetarian (Kerala) meal-plan template ----
    // ~140 g protein on training days; Saturday is a gentle one-meal fasting day.

    private static void SeedDiet(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DietDay>().HasData(
            DietD(1, DayOfWeek.Sunday, false, "Pull day — fuel the back workout."),
            DietD(2, DayOfWeek.Monday, false, "Leg day — carbs around training."),
            DietD(3, DayOfWeek.Tuesday, false, "Rest day — keep protein high for recovery."),
            DietD(4, DayOfWeek.Wednesday, false, "Metabolic day — light, protein-forward."),
            DietD(5, DayOfWeek.Thursday, false, "Mobility day — hydration and greens."),
            DietD(6, DayOfWeek.Friday, false, "Rest day — recovery and protein."),
            DietD(7, DayOfWeek.Saturday, true, "Gentle fasting day: one substantial protein-forward meal + fluids (buttermilk, water). Not a hard water-only fast — eases the gut while keeping protein up. Consult a doctor if anything feels off.")
        );

        modelBuilder.Entity<Meal>().HasData(
            // Sunday
            Ml(1, 1, MealSlot.Breakfast, "7:30 AM", "Puttu + black chickpea (kadala) curry + 2 boiled eggs", 30, 1),
            Ml(2, 1, MealSlot.Lunch, "1:00 PM", "Rice + toor-dal sambar + soya-chunk stir fry + curd", 40, 2),
            Ml(3, 1, MealSlot.Snack, "4:30 PM", "Greek yogurt + roasted peanuts + banana", 22, 3),
            Ml(4, 1, MealSlot.Dinner, "8:00 PM", "Chapathi + green-gram (cherupayar) curry + veg thoran", 28, 4),
            // Monday
            Ml(5, 2, MealSlot.Breakfast, "7:30 AM", "Oats cooked in milk + peanut butter + 1 boiled egg", 28, 1),
            Ml(6, 2, MealSlot.Lunch, "1:00 PM", "Rice + moong-dal + soya curry + beetroot thoran + curd", 40, 2),
            Ml(7, 2, MealSlot.Snack, "4:30 PM", "Buttermilk + roasted chana (chickpea)", 18, 3),
            Ml(8, 2, MealSlot.Dinner, "8:00 PM", "Chapathi + chickpea masala + mixed vegetables", 30, 4),
            // Tuesday (rest)
            Ml(9, 3, MealSlot.Breakfast, "7:30 AM", "Idli + sambar + 2 boiled eggs", 26, 1),
            Ml(10, 3, MealSlot.Lunch, "1:00 PM", "Rice + horse-gram (muthira) curry + spinach thoran + curd", 34, 2),
            Ml(11, 3, MealSlot.Snack, "4:30 PM", "Greek yogurt + mixed nuts", 20, 3),
            Ml(12, 3, MealSlot.Dinner, "8:00 PM", "Chapathi + soya-chunk curry + salad", 32, 4),
            // Wednesday (metabolic)
            Ml(13, 4, MealSlot.Breakfast, "7:30 AM", "Overnight oats + Greek yogurt + peanut butter + banana", 30, 1),
            Ml(14, 4, MealSlot.Lunch, "1:00 PM", "Rice + mixed-dal + chickpea sundal + curd", 36, 2),
            Ml(15, 4, MealSlot.Snack, "post-workout", "Boiled green gram + 2 boiled eggs", 24, 3),
            Ml(16, 4, MealSlot.Dinner, "8:00 PM", "Chapathi + vegetable + lentil stew ( Kerala ishtu )", 26, 4),
            // Thursday (mobility)
            Ml(17, 5, MealSlot.Breakfast, "7:30 AM", "Appam + green-gram curry + 1 boiled egg", 24, 1),
            Ml(18, 5, MealSlot.Lunch, "1:00 PM", "Rice + sambar + soya stir fry + curd", 38, 2),
            Ml(19, 5, MealSlot.Snack, "4:30 PM", "Greek yogurt + peanuts", 20, 3),
            Ml(20, 5, MealSlot.Dinner, "8:00 PM", "Chapathi + rajma / chickpea curry + greens", 30, 4),
            // Friday (rest)
            Ml(21, 6, MealSlot.Breakfast, "7:30 AM", "Oats + milk + 2 boiled eggs", 28, 1),
            Ml(22, 6, MealSlot.Lunch, "1:00 PM", "Rice + dal + soya curry + curd", 38, 2),
            Ml(23, 6, MealSlot.Snack, "4:30 PM", "Buttermilk + roasted chana + fruit", 16, 3),
            Ml(24, 6, MealSlot.Dinner, "8:00 PM", "Chapathi + mixed-bean curry + vegetables", 30, 4),
            // Saturday (fasting — one meal)
            Ml(25, 7, MealSlot.Lunch, "1:00 PM", "One large plate: rice + mixed-dal + chickpea + soya + curd + vegetables", 45, 1),
            Ml(26, 7, MealSlot.Snack, "through the day", "Fluids only — water, buttermilk, black coffee/tea", 4, 2)
        );
    }

    // ---- Builders ----

    // Seeded WorkoutDay/Exercise templates live in a high Id range so they never
    // collide with user-owned rows (which auto-increment from low Ids) when this
    // reseed migration is applied over an existing database.
    private const int SeedBase = 9000;

    private static WorkoutDay Day(int id, string name, string focus, int sortOrder, DayOfWeek day)
        => new() { Id = SeedBase + id, Name = name, Focus = focus, SortOrder = sortOrder, ScheduledDayOfWeek = day };

    private static Muscle Mu(int id, string name, string group, string function, string injury)
        => new() { Id = id, Name = name, Group = group, Function = function, InjuryNotes = injury };

    private static EquipmentItem Eq(int id, string name, EquipmentCategory cat, string specs, EquipmentStatus status, int sortOrder)
        => new() { Id = id, Name = name, Category = cat, Specs = specs, Status = status, SortOrder = sortOrder };

    private static DietDay DietD(int id, DayOfWeek day, bool fasting, string notes)
        => new() { Id = id, DayOfWeek = day, IsFastingDay = fasting, Notes = notes };

    private static Meal Ml(int id, int dayId, MealSlot slot, string timeHint, string items, int proteinG, int sortOrder)
        => new() { Id = id, DietDayId = dayId, Slot = slot, TimeHint = timeHint, Items = items, ProteinG = proteinG, SortOrder = sortOrder };

    private static Exercise Ex(
        int id, int dayId, int sortOrder, string name, int? low, int? high, string display, int baseSets,
        string primaryMuscles, string? secondaryMuscles, string cue,
        string? benefits = null, string? mistakes = null, string? safety = null,
        int? con = null, int? ecc = null, int? pause = null,
        bool hold = false, string? holdBreath = null)
        => new()
        {
            Id = SeedBase + id,
            WorkoutDayId = SeedBase + dayId,
            Name = name,
            TargetRepsLow = low,
            TargetRepsHigh = high,
            RepsDisplay = display,
            BaseSets = baseSets,
            SortOrder = sortOrder,
            Cue = cue,
            VideoId = null,
            PrimaryMuscles = primaryMuscles,
            SecondaryMuscles = secondaryMuscles,
            Benefits = benefits,
            CommonMistakes = mistakes,
            SafetyTips = safety,
            AnimationRef = Slug(name),
            // Timed holds get steady-breathing guidance; dynamic lifts exhale on the
            // effort (concentric) and inhale on the return (eccentric), with a
            // structured tempo that drives the per-rep metronome on the work screen.
            BreathingConcentric = hold ? null : "Exhale",
            BreathingEccentric = hold ? null : "Inhale",
            BreathingNotes = hold ? (holdBreath ?? "Breathe slow and steady — don't hold your breath.") : null,
            TempoConcentricSeconds = hold ? null : con,
            TempoEccentricSeconds = hold ? null : ecc,
            TempoPauseSeconds = hold ? null : pause,
            Tempo = hold || con is null || ecc is null
                ? null
                : $"Up {con}s · Down {ecc}s" + (pause is > 0 ? $" · {pause}s pause" : string.Empty),
        };

    /// <summary>Stable kebab-case key for the animation asset registry (resolved client-side).</summary>
    private static string Slug(string name)
    {
        var chars = name.ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray();
        var slug = new string(chars);
        while (slug.Contains("--")) slug = slug.Replace("--", "-");
        return slug.Trim('-');
    }
}
