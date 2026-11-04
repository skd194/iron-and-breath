using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IronAndBreath.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Revamp_Program_Equipment_Nutrition_Muscles_Tempo_Schedule_Archive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "WorkoutDays",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ScheduledDayOfWeek",
                table: "WorkoutDays",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TempoConcentricSeconds",
                table: "Exercises",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TempoEccentricSeconds",
                table: "Exercises",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TempoPauseSeconds",
                table: "Exercises",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BodyMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    WeightKg = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BodyMetrics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DietDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFastingDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DietDays_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    Specs = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Muscles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Group = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Function = table.Column<string>(type: "TEXT", maxLength: 600, nullable: false),
                    InjuryNotes = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muscles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NutritionProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeightKg = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    ProteinTargetG = table.Column<int>(type: "INTEGER", nullable: false),
                    CalorieTarget = table.Column<int>(type: "INTEGER", nullable: false),
                    DietStyle = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutritionProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DietDayId = table.Column<int>(type: "INTEGER", nullable: false),
                    Slot = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeHint = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    Items = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    ProteinG = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meals_DietDays_DietDayId",
                        column: x => x.DietDayId,
                        principalTable: "DietDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DietDays",
                columns: new[] { "Id", "DayOfWeek", "IsFastingDay", "Notes", "UserId" },
                values: new object[,]
                {
                    { 1, 0, false, "Pull day — fuel the back workout.", null },
                    { 2, 1, false, "Leg day — carbs around training.", null },
                    { 3, 2, false, "Rest day — keep protein high for recovery.", null },
                    { 4, 3, false, "Metabolic day — light, protein-forward.", null },
                    { 5, 4, false, "Mobility day — hydration and greens.", null },
                    { 6, 5, false, "Rest day — recovery and protein.", null },
                    { 7, 6, true, "Gentle fasting day: one substantial protein-forward meal + fluids (buttermilk, water). Not a hard water-only fast — eases the gut while keeping protein up. Consult a doctor if anything feels off.", null }
                });

            migrationBuilder.InsertData(
                table: "EquipmentItems",
                columns: new[] { "Id", "Category", "Name", "SortOrder", "Specs", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, 0, "Adjustable Bench", 1, "Incline / decline / flat, 250 kg capacity, resistance ropes included.", 0, null },
                    { 2, 1, "Straight Barbell (5 ft, 25 mm)", 2, "For bench press, squats, rows and deadlifts.", 0, null },
                    { 3, 1, "EZ Curl Rod (3 ft)", 3, "For curls and overhead triceps extensions.", 0, null },
                    { 4, 2, "Dumbbell Handles (pair, 14 in)", 4, "Loadable dumbbell rods with collars.", 0, null },
                    { 5, 3, "Weight Plates", 5, "4 × 2.5 kg + 4 × 2 kg rubber plates (18 kg total).", 0, null },
                    { 6, 5, "Resistance Ropes", 6, "Attach to the bench for face pulls and presses.", 0, null },
                    { 7, 6, "Skipping Rope", 7, "Warm-ups and conditioning intervals.", 0, null },
                    { 8, 3, "Heavier Plates", 8, "Add 5 kg / 10 kg plates as the lifts get strong.", 1, null }
                });

            migrationBuilder.InsertData(
                table: "Muscles",
                columns: new[] { "Id", "Function", "Group", "InjuryNotes", "Name" },
                values: new object[,]
                {
                    { 1, "The pectorals push things away from you and across your body — pressing, pushing doors, throwing.", "Chest", "Warm up the shoulders; avoid over-flaring the elbows when pressing.", "Chest" },
                    { 2, "The clavicular head of the chest drives pressing on an incline and from low to high.", "Chest", "Don't set incline benches too steep or the shoulders take over.", "Upper Chest" },
                    { 3, "The three-headed muscle on the back of the arm straightens the elbow — every push and lockout.", "Arms", "Control overhead extensions; the long head is easy to strain cold.", "Triceps" },
                    { 4, "The deltoids raise and press the arm overhead and out to the sides — everything overhead.", "Shoulders", "Press pain-free ranges only; the shoulder is the body's most mobile, least stable joint.", "Shoulders" },
                    { 5, "The rear deltoids pull the arm back and support posture — the antidote to hunching.", "Shoulders", "Train light and controlled; heavy swinging strains the shoulder.", "Rear Delts" },
                    { 6, "The trapezius shrugs, rotates and stabilises the shoulder blades and supports the neck.", "Back", "Avoid over-shrugging during raises; keep the neck long.", "Traps" },
                    { 7, "The mid-back muscles pull, row and hold you upright with good posture.", "Back", "Keep a neutral spine when rowing; never round under load.", "Back" },
                    { 8, "The rhomboids and mid-traps retract the shoulder blades and keep the chest open.", "Back", "Strengthen to counter desk posture; move slowly in stretches.", "Upper Back" },
                    { 9, "The latissimus dorsi pulls the arms down and back — pull-ups, rows, the V-taper.", "Back", "Keep the shoulder down and back; don't let it roll forward at the stretch.", "Lats" },
                    { 10, "The biceps bend the elbow and turn the palm up — curling and pulling.", "Arms", "Avoid swinging; jerking loads strains the elbow tendon.", "Biceps" },
                    { 11, "The forearm muscles grip, and bend and rotate the wrist — grip strength and carrying.", "Arms", "Build grip gradually; wrist tendons flare with sudden volume.", "Forearms" },
                    { 12, "The spinal erectors hold you upright and extend the spine — hinging, lifting, standing tall.", "Core", "Brace and hinge correctly; most back pain comes from rounding under load.", "Lower Back" },
                    { 13, "The long muscles either side of the spine extend and stabilise the back through every lift.", "Core", "Keep a neutral spine; strengthen them to relieve tightness.", "Spinal Erectors" },
                    { 14, "The glutes extend the hip and drive you forward — standing, squatting, sprinting, jumping.", "Legs", "Activate them so the lower back doesn't take over hinging.", "Glutes" },
                    { 15, "The abdominals brace the trunk and resist bending and twisting — they protect the spine in every lift.", "Core", "Train bracing, not just crunching; a strong core spares the back.", "Core" },
                    { 16, "The side-abdominals rotate and side-bend the trunk and resist unwanted twisting.", "Core", "Rotate from the ribcage, not the lower back.", "Obliques" },
                    { 17, "The front-thigh muscles straighten the knee — squatting, standing, climbing stairs.", "Legs", "Keep the knee tracking over the foot; don't let it cave in.", "Quadriceps" },
                    { 18, "The back-thigh muscles bend the knee and extend the hip — hinging, running, bracing the knee.", "Legs", "Stretch and strengthen; tight hamstrings tug on the lower back.", "Hamstrings" },
                    { 19, "The calves point the foot and drive push-off — walking, running, jumping.", "Legs", "Train a full stretch; respond best to high reps and a pause.", "Calves" },
                    { 20, "The hip flexors lift the thigh toward the torso — walking, running, raising the legs.", "Legs", "Stretch them regularly; sitting keeps them tight and tilts the pelvis.", "Hip Flexors" },
                    { 21, "The neck muscles support and move the head and anchor the upper traps.", "Neck", "Use only gentle pressure when stretching; the neck is delicate.", "Neck" }
                });

            migrationBuilder.InsertData(
                table: "WorkoutDays",
                columns: new[] { "Id", "Focus", "IsArchived", "Name", "ScheduledDayOfWeek", "SortOrder", "UserId" },
                values: new object[,]
                {
                    { 9001, "Horizontal + overhead pressing", false, "Push · Chest / Shoulders / Triceps", 6, 1, null },
                    { 9002, "Rows, pulls, curls, back health", false, "Pull · Back / Biceps / Rear Delts", 0, 2, null },
                    { 9003, "Squat, hinge, lunge, trunk", false, "Legs + Core", 1, 3, null },
                    { 9004, "Compound circuit with skipping", false, "Full-Body Metabolic + Cardio", 3, 4, null },
                    { 9005, "Full-body stretch, back-focused recovery", false, "Mobility & Yoga", 4, 5, null }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "AnimationRef", "BaseSets", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds", "VideoId", "WorkoutDayId" },
                values: new object[,]
                {
                    { 9001, "flat-barbell-bench-press", 4, "The foundational horizontal press for chest and triceps mass.", "Exhale", "Inhale", null, "Flaring elbows to 90°, bouncing the bar off the chest.", "Shoulder blades pinched, bar to mid-chest, drive up and slightly back.", "Flat Barbell Bench Press", "Chest,Triceps", "4 sets x 8-12 reps", "Keep wrists stacked over elbows; use a spotter or safety at light-but-heavy loads.", "Shoulders", 1, 12, 8, "Up 1s · Down 3s", 1, 3, null, null, 9001 },
                    { 9002, "incline-dumbbell-press", 3, "Targets the upper chest the flat press under-trains.", "Exhale", "Inhale", null, "Setting the bench too steep (turns it into a shoulder press).", "Bench at 30°, press the dumbbells up and together without clanging.", "Incline Dumbbell Press", "Upper Chest,Shoulders", "3 sets x 10-12 reps", "Control the dumbbells at the bottom; don't let shoulders roll forward.", "Triceps", 2, 12, 10, "Up 1s · Down 2s", 1, 2, null, null, 9001 },
                    { 9003, "incline-dumbbell-flye", 3, "Isolates the chest through a long stretch for extra hypertrophy.", "Exhale", "Inhale", null, "Bending the elbows into a press, going too heavy.", "Soft elbows, wide arc, feel the stretch, squeeze at the top.", "Incline Dumbbell Flye", "Chest", "3 sets x 12-15 reps", "Keep a slight elbow bend throughout to protect the shoulder joint.", "Shoulders", 3, 15, 12, "Up 1s · Down 3s", 1, 3, null, null, 9001 },
                    { 9004, "seated-dumbbell-shoulder-press", 3, "Builds overhead pressing strength and delt size.", "Exhale", "Inhale", null, "Over-arching the back, pressing with a forward lean.", "Brace the core, press overhead without arching the lower back.", "Seated Dumbbell Shoulder Press", "Shoulders,Triceps", "3 sets x 10-12 reps", "Keep ribs down; stop the range where shoulders feel pinch-free.", "Upper Chest", 4, 12, 10, "Up 1s · Down 2s", 1, 2, null, null, 9001 },
                    { 9005, "lateral-raise", 3, "Widens the shoulders by isolating the side delts.", "Exhale", "Inhale", null, "Swinging the weight, shrugging the traps up.", "Lead with the elbows, raise to shoulder height only, control down.", "Lateral Raise", "Shoulders", "3 sets x 15-20 reps", "Go light; momentum here strains the shoulder and neck.", "Traps", 5, 20, 15, "Up 1s · Down 2s", 1, 2, null, null, 9001 },
                    { 9006, "overhead-triceps-extension", 3, "Hits the long head of the triceps through a deep stretch.", "Exhale", "Inhale", null, "Flaring the elbows out, using the shoulders to lift.", "Elbows tucked and pointing forward, full stretch behind the head.", "Overhead Triceps Extension", "Triceps", "3 sets x 12-15 reps", "Lower under control; don't let the weight yank the elbow open.", "Shoulders", 6, 15, 12, "Up 1s · Down 3s", 1, 3, null, null, 9001 },
                    { 9007, "push-ups", 2, "A bodyweight finisher to fully fatigue the chest and triceps.", "Exhale", "Inhale", null, "Sagging hips, half-range reps.", "Straight line head-to-heels, full range, chest to the floor.", "Push-ups", "Chest,Triceps", "2 sets to near-failure", "Keep the core braced to protect the lower back.", "Shoulders,Core", 7, null, null, "Up 1s · Down 2s", 1, 2, null, null, 9001 },
                    { 9008, "barbell-bent-over-row", 4, "The primary mass builder for the whole back.", "Exhale", "Inhale", null, "Rounding the back, jerking with the hips.", "Hinge to ~45°, flat back, pull the bar to the lower ribs.", "Barbell Bent-Over Row", "Back,Lats", "4 sets x 8-12 reps", "Keep a neutral spine; brace hard to protect the lower back.", "Biceps,Rear Delts", 1, 12, 8, "Up 1s · Down 2s", 1, 2, null, null, 9002 },
                    { 9009, "one-arm-dumbbell-row", 3, "Unilateral row that fixes left/right imbalances.", "Exhale", "Inhale", null, "Twisting the torso to heave the weight up.", "Hand and knee on the bench, drive the elbow to the hip, squeeze the lat.", "One-Arm Dumbbell Row", "Lats,Back", "3 sets x 10-12 reps/side", "Keep the spine neutral and hips square.", "Biceps", 2, 12, 10, "Up 1s · Down 2s", 1, 2, null, null, 9002 },
                    { 9010, "resistance-rope-face-pull", 3, "Bulletproofs the shoulders and improves posture.", "Exhale", "Inhale", null, "Using too much weight and turning it into a row.", "Pull the ropes to your forehead, elbows high, squeeze the rear delts.", "Resistance-Rope Face Pull", "Rear Delts,Traps", "3 sets x 15-20 reps", "Keep it light and controlled; this is a health movement.", "Back", 3, 20, 15, "Up 1s · Down 2s", 1, 2, null, null, 9002 },
                    { 9011, "barbell-curl", 3, "The classic mass builder for the biceps.", "Exhale", "Inhale", null, "Swinging the torso, using the shoulders to lift.", "Elbows pinned to the sides, curl the curl-rod, no swinging.", "Barbell Curl", "Biceps", "3 sets x 10-12 reps", "Keep wrists neutral; the EZ bar is easier on them.", "Forearms", 4, 12, 10, "Up 1s · Down 3s", 1, 3, null, null, 9002 },
                    { 9012, "hammer-curl", 3, "Builds the brachialis for thicker arms and strong forearms.", "Exhale", "Inhale", null, "Rushing the lowering phase.", "Neutral grip (palms facing in), control both directions.", "Hammer Curl", "Biceps,Forearms", "3 sets x 12-15 reps", "No swinging; keep the elbows still.", null, 5, 15, 12, "Up 1s · Down 2s", 1, 2, null, null, 9002 },
                    { 9013, "rear-delt-flye", 3, "Balances the shoulders and improves posture.", "Exhale", "Inhale", null, "Using the lower back to swing the weight.", "Hinge forward, raise the dumbbells wide, lead with the pinkies.", "Rear-Delt Flye", "Rear Delts", "3 sets x 15-20 reps", "Light weight, slow tempo; protect the lower back by bracing.", "Traps,Back", 6, 20, 15, "Up 1s · Down 2s", 1, 2, null, null, 9002 },
                    { 9014, "superman-back-extension-hold", 3, "Strengthens the spinal erectors to relieve back tightness.", null, null, "Breathe slow and steady — don't hold your breath.", "Cranking the neck back.", "Lift chest and thighs off the floor, squeeze the glutes, breathe.", "Superman / Back Extension Hold", "Lower Back,Glutes", "3 sets x 30-45 sec", "Keep the neck neutral; stop if it pinches the lower back.", "Core", 7, null, null, null, null, null, null, null, 9002 },
                    { 9015, "barbell-back-squat", 4, "The king of lower-body mass and strength.", "Exhale", "Inhale", null, "Knees caving in, heels lifting, rounding the back.", "Chest up, sit between the hips, knees track the toes, drive up.", "Barbell Back Squat", "Quadriceps,Glutes", "4 sets x 8-12 reps", "Brace the core; keep depth where your back stays neutral.", "Hamstrings,Core", 1, 12, 8, "Up 1s · Down 3s", 1, 3, null, null, 9003 },
                    { 9016, "romanian-deadlift", 4, "Builds the hamstrings and glutes and protects the back.", "Exhale", "Inhale", null, "Rounding the back, squatting instead of hinging.", "Hinge at the hips, soft knees, bar close, feel the hamstring stretch.", "Romanian Deadlift", "Hamstrings,Glutes", "4 sets x 10-12 reps", "Keep the bar against the legs and the spine neutral throughout.", "Lower Back", 2, 12, 10, "Up 1s · Down 3s", 1, 3, null, null, 9003 },
                    { 9017, "bulgarian-split-squat", 3, "Unilateral leg builder that also trains balance.", "Exhale", "Inhale", null, "Leaning too far forward, letting the front knee cave.", "Rear foot on the bench, drop straight down, drive through the front heel.", "Bulgarian Split Squat", "Quadriceps,Glutes", "3 sets x 10-12 reps/leg", "Keep the front knee tracking over the foot.", "Hamstrings", 3, 12, 10, "Up 1s · Down 2s", 1, 2, null, null, 9003 },
                    { 9018, "walking-lunge", 3, "Trains the legs through a long, athletic range.", "Exhale", "Inhale", null, "Short steps that overload the knee.", "Long stride, back knee toward the floor, torso tall.", "Walking Lunge", "Quadriceps,Glutes", "3 sets x 12 reps/leg", "Step long enough that the front shin stays near vertical.", "Hamstrings,Calves", 4, 12, 12, "Up 1s · Down 2s", 1, 2, null, null, 9003 },
                    { 9019, "standing-calf-raise", 4, "Builds the calves, which need high reps and a strong stretch.", "Exhale", "Inhale", null, "Bouncing through a short range.", "Full stretch at the bottom, pause and squeeze at the top.", "Standing Calf Raise", "Calves", "4 sets x 20 reps", "Control the bottom stretch; don't ballistically drop.", null, 5, 20, 20, "Up 1s · Down 2s · 1s pause", 1, 2, 1, null, 9003 },
                    { 9020, "plank-hold", 3, "Builds deep core stability that protects the spine.", null, null, "Breathe slow and steady — don't hold your breath.", "Hips sagging or piking up.", "Squeeze the glutes and abs, straight line, no sagging hips.", "Plank Hold", "Core", "3 sets x 45-60 sec", "Keep a neutral neck and stop before the lower back sags.", "Shoulders,Glutes", 6, null, null, null, null, null, null, null, 9003 },
                    { 9021, "lying-leg-raise", 3, "Targets the lower abs.", "Exhale", "Inhale", null, "Arching the lower back off the floor.", "Press the lower back down, raise the legs, lower slowly.", "Lying Leg Raise", "Core", "3 sets x 15 reps", "Keep the lower back pinned down the whole set.", "Hip Flexors", 7, 15, 15, "Up 1s · Down 2s", 1, 2, null, null, 9003 },
                    { 9022, "skipping-warm-up-intervals", 1, "Raises the heart rate and primes the whole body.", null, null, "Breathe rhythmically with the rope.", "Jumping too high and landing hard.", "Light bounces on the balls of the feet, relaxed shoulders.", "Skipping — Warm-up Intervals", "Calves,Shoulders", "3-5 min easy pace", "Land softly through the balls of the feet to spare the knees.", "Core", 1, null, null, null, null, null, null, null, 9004 },
                    { 9023, "dumbbell-thruster", 4, "A full-body compound that burns a lot in little time.", "Exhale", "Inhale", null, "Pressing before the legs finish driving.", "Squat, then drive up and press overhead in one fluid motion.", "Dumbbell Thruster", "Quadriceps,Shoulders", "4 sets x 10-12 reps", "Keep the core braced; don't arch the back at lockout.", "Glutes,Triceps,Core", 2, 12, 10, "Up 1s · Down 2s", 1, 2, null, null, 9004 },
                    { 9024, "dumbbell-deadlift", 3, "Trains the posterior chain safely with lighter load.", "Exhale", "Inhale", null, "Rounding the back, jerking off the floor.", "Neutral spine, push the floor away, lock out tall.", "Dumbbell Deadlift", "Hamstrings,Glutes", "3 sets x 12 reps", "Hinge, don't squat; keep the spine neutral.", "Back,Core", 3, 12, 12, "Up 1s · Down 2s", 1, 2, null, null, 9004 },
                    { 9025, "renegade-row", 3, "Combines a row with a hard anti-rotation core challenge.", "Exhale", "Inhale", null, "Twisting the hips toward the working arm.", "Wide feet for stability, row one dumbbell, minimise hip rotation.", "Renegade Row", "Back,Core", "3 sets x 10 reps/side", "Set feet wide and keep the hips level.", "Biceps,Shoulders", 4, 10, 10, "Up 1s · Down 2s", 1, 2, null, null, 9004 },
                    { 9026, "push-ups", 3, "Bodyweight pressing volume for conditioning.", "Exhale", "Inhale", null, "Sagging hips, partial reps.", "Straight line head-to-heels, full range of motion.", "Push-ups", "Chest,Triceps", "3 sets to near-failure", "Brace the core to protect the lower back.", "Shoulders,Core", 5, null, null, "Up 1s · Down 2s", 1, 2, null, null, 9004 },
                    { 9027, "russian-twist", 3, "Builds rotational core strength and the obliques.", "Exhale", "Inhale", null, "Moving only the arms, not the torso.", "Rotate from the trunk, touch each side, feet hovering if able.", "Russian Twist", "Core,Obliques", "3 sets x 20 reps", "Rotate from the ribcage, not by yanking the lower back.", "Hip Flexors", 6, 20, 20, "Up 1s · Down 1s", 1, 1, null, null, 9004 },
                    { 9028, "skipping-finisher", 1, "Caps the session with a conditioning burst.", null, null, "Breathe rhythmically with the rope.", "Tensing the shoulders.", "Steady rhythm, pick up the pace for the last 30 seconds.", "Skipping — Finisher", "Calves,Shoulders", "2-3 min, build pace", "Keep landings soft; stop if the calves cramp.", "Core", 7, null, null, null, null, null, null, null, 9004 },
                    { 9029, "cat-cow", 1, "Mobilises the whole spine and eases back stiffness.", null, null, "Inhale to arch, exhale to round.", "Rushing; forcing the range.", "Alternate arching and rounding the spine with the breath.", "Cat-Cow", "Spinal Erectors,Core", "60 sec, slow flow", "Move only as far as comfortable.", "Lower Back", 1, null, null, null, null, null, null, null, 9005 },
                    { 9030, "child-s-pose", 1, "Gently stretches the back and lats and calms the nervous system.", null, null, "Breathe deep into the back of the ribs.", "Holding tension in the shoulders.", "Hips back to the heels, arms long, let the back decompress.", "Child's Pose", "Lats,Lower Back", "60 sec hold", "Widen the knees if the belly or hips feel cramped.", "Shoulders", 2, null, null, null, null, null, null, null, 9005 },
                    { 9031, "downward-dog", 1, "Stretches the whole back line — calves, hamstrings, spine.", null, null, "Breathe slow and steady.", "Rounding the back to force the heels down.", "Hips high, heels reaching down, spine long.", "Downward Dog", "Hamstrings,Calves", "45 sec hold", "Bend the knees as needed to keep the spine long.", "Shoulders,Back", 3, null, null, null, null, null, null, null, 9005 },
                    { 9032, "cobra-sphinx", 1, "Counters hunching and gently extends the lower back.", null, null, "Breathe into the chest.", "Cranking the neck or over-extending.", "Lift the chest, elbows under shoulders, shoulders down away from ears.", "Cobra / Sphinx", "Lower Back,Core", "45 sec hold", "Keep the lift gentle; no sharp lower-back pinch.", "Chest", 4, null, null, null, null, null, null, null, 9005 },
                    { 9033, "thread-the-needle", 1, "Opens the upper back and shoulders.", null, null, "Breathe slow and steady.", "Collapsing the supporting shoulder.", "From all fours, thread one arm under the other, rest on the shoulder.", "Thread-the-Needle", "Upper Back,Rear Delts", "45 sec/side", "Keep weight evenly distributed; move slowly.", "Shoulders", 5, null, null, null, null, null, null, null, 9005 },
                    { 9034, "supine-spinal-twist", 1, "Releases the lower back and obliques — great for tightness.", null, null, "Exhale as you sink deeper.", "Forcing the knees down and lifting the shoulder.", "On your back, drop the knees to one side, arms wide, gaze opposite.", "Supine Spinal Twist", "Lower Back,Obliques", "45 sec/side", "Keep both shoulders on the floor; let gravity do the work.", "Glutes,Spinal Erectors", 6, null, null, null, null, null, null, null, 9005 },
                    { 9035, "seated-forward-fold", 1, "Lengthens the hamstrings and the whole back line.", null, null, "Exhale to fold a little deeper.", "Rounding hard from the upper back to reach further.", "Hinge from the hips, long spine, reach toward the feet.", "Seated Forward Fold", "Hamstrings,Lower Back", "60 sec hold", "Bend the knees slightly; lead with the chest, not the head.", "Calves", 7, null, null, null, null, null, null, null, 9005 },
                    { 9036, "pigeon-pose", 1, "Opens tight hips and glutes that pull on the lower back.", null, null, "Breathe slow and steady.", "Collapsing onto one hip.", "Front shin angled, back leg long, fold over the front leg.", "Pigeon Pose", "Glutes,Hip Flexors", "45 sec/side", "Prop a cushion under the hip to keep it level.", "Lower Back", 8, null, null, null, null, null, null, null, 9005 },
                    { 9037, "standing-hamstring-hip-flexor-stretch", 1, "Balances the front and back of the hips for a neutral pelvis.", null, null, "Breathe slow and steady.", "Bouncing into the stretch.", "Stretch one hamstring, then lunge to open the opposite hip flexor.", "Standing Hamstring + Hip-Flexor Stretch", "Hamstrings,Hip Flexors", "45 sec/side", "Ease in gradually; never bounce.", "Quadriceps", 9, null, null, null, null, null, null, null, 9005 },
                    { 9038, "neck-shoulder-release", 1, "Releases desk-and-training tension in the neck and traps.", null, null, "Breathe slow and steady.", "Pulling the head hard with the hand.", "Gently tilt the ear to the shoulder, roll the shoulders back and down.", "Neck & Shoulder Release", "Traps,Neck", "45 sec hold", "Use only light pressure; the neck is delicate.", "Shoulders", 10, null, null, null, null, null, null, null, 9005 }
                });

            migrationBuilder.InsertData(
                table: "Meals",
                columns: new[] { "Id", "DietDayId", "Items", "ProteinG", "Slot", "SortOrder", "TimeHint" },
                values: new object[,]
                {
                    { 1, 1, "Puttu + black chickpea (kadala) curry + 2 boiled eggs", 30, 0, 1, "7:30 AM" },
                    { 2, 1, "Rice + toor-dal sambar + soya-chunk stir fry + curd", 40, 1, 2, "1:00 PM" },
                    { 3, 1, "Greek yogurt + roasted peanuts + banana", 22, 2, 3, "4:30 PM" },
                    { 4, 1, "Chapathi + green-gram (cherupayar) curry + veg thoran", 28, 3, 4, "8:00 PM" },
                    { 5, 2, "Oats cooked in milk + peanut butter + 1 boiled egg", 28, 0, 1, "7:30 AM" },
                    { 6, 2, "Rice + moong-dal + soya curry + beetroot thoran + curd", 40, 1, 2, "1:00 PM" },
                    { 7, 2, "Buttermilk + roasted chana (chickpea)", 18, 2, 3, "4:30 PM" },
                    { 8, 2, "Chapathi + chickpea masala + mixed vegetables", 30, 3, 4, "8:00 PM" },
                    { 9, 3, "Idli + sambar + 2 boiled eggs", 26, 0, 1, "7:30 AM" },
                    { 10, 3, "Rice + horse-gram (muthira) curry + spinach thoran + curd", 34, 1, 2, "1:00 PM" },
                    { 11, 3, "Greek yogurt + mixed nuts", 20, 2, 3, "4:30 PM" },
                    { 12, 3, "Chapathi + soya-chunk curry + salad", 32, 3, 4, "8:00 PM" },
                    { 13, 4, "Overnight oats + Greek yogurt + peanut butter + banana", 30, 0, 1, "7:30 AM" },
                    { 14, 4, "Rice + mixed-dal + chickpea sundal + curd", 36, 1, 2, "1:00 PM" },
                    { 15, 4, "Boiled green gram + 2 boiled eggs", 24, 2, 3, "post-workout" },
                    { 16, 4, "Chapathi + vegetable + lentil stew ( Kerala ishtu )", 26, 3, 4, "8:00 PM" },
                    { 17, 5, "Appam + green-gram curry + 1 boiled egg", 24, 0, 1, "7:30 AM" },
                    { 18, 5, "Rice + sambar + soya stir fry + curd", 38, 1, 2, "1:00 PM" },
                    { 19, 5, "Greek yogurt + peanuts", 20, 2, 3, "4:30 PM" },
                    { 20, 5, "Chapathi + rajma / chickpea curry + greens", 30, 3, 4, "8:00 PM" },
                    { 21, 6, "Oats + milk + 2 boiled eggs", 28, 0, 1, "7:30 AM" },
                    { 22, 6, "Rice + dal + soya curry + curd", 38, 1, 2, "1:00 PM" },
                    { 23, 6, "Buttermilk + roasted chana + fruit", 16, 2, 3, "4:30 PM" },
                    { 24, 6, "Chapathi + mixed-bean curry + vegetables", 30, 3, 4, "8:00 PM" },
                    { 25, 7, "One large plate: rice + mixed-dal + chickpea + soya + curd + vegetables", 45, 1, 1, "1:00 PM" },
                    { 26, 7, "Fluids only — water, buttermilk, black coffee/tea", 4, 2, 2, "through the day" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyMetrics_UserId_Date",
                table: "BodyMetrics",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_DietDays_UserId",
                table: "DietDays",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentItems_UserId",
                table: "EquipmentItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_DietDayId_SortOrder",
                table: "Meals",
                columns: new[] { "DietDayId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Muscles_Name",
                table: "Muscles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NutritionProfiles_UserId",
                table: "NutritionProfiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyMetrics");

            migrationBuilder.DropTable(
                name: "EquipmentItems");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "Muscles");

            migrationBuilder.DropTable(
                name: "NutritionProfiles");

            migrationBuilder.DropTable(
                name: "DietDays");

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9001);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9002);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9003);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9004);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9005);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9006);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9007);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9008);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9009);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9010);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9011);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9012);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9013);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9014);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9015);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9016);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9017);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9018);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9019);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9020);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9021);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9022);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9023);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9024);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9025);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9026);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9027);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9028);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9029);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9030);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9031);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9032);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9033);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9034);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9035);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9036);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9037);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9038);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 9001);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 9002);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 9003);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 9004);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 9005);

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "WorkoutDays");

            migrationBuilder.DropColumn(
                name: "ScheduledDayOfWeek",
                table: "WorkoutDays");

            migrationBuilder.DropColumn(
                name: "TempoConcentricSeconds",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "TempoEccentricSeconds",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "TempoPauseSeconds",
                table: "Exercises");

            migrationBuilder.InsertData(
                table: "WorkoutDays",
                columns: new[] { "Id", "Focus", "Name", "SortOrder", "UserId" },
                values: new object[,]
                {
                    { 1, "Chest, shoulders, triceps", "Upper Body (Push)", 1, null },
                    { 2, "Quads, hamstrings, glutes, core", "Lower Body + Core", 2, null },
                    { 3, "Back, biceps, rear delts", "Upper Body (Pull)", 3, null },
                    { 4, "Total body, conditioning, core", "Full Body (Metabolic)", 4, null }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "AnimationRef", "BaseSets", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "VideoId", "WorkoutDayId" },
                values: new object[,]
                {
                    { 1, null, 4, null, "Exhale", "Inhale", null, null, "Lower elbows to the floor, pause, press up.", "Dumbbell Floor Press", "Chest,Triceps", "4 sets x 10-12 reps", null, "Shoulders", 1, 12, 10, null, null, 1 },
                    { 2, null, 3, null, "Exhale", "Inhale", null, null, "Brace core, press overhead without arching the back.", "Dumbbell Shoulder Press", "Shoulders,Triceps", "3 sets x 10-12 reps", null, "Upper Chest", 2, 12, 10, null, null, 1 },
                    { 3, null, 3, null, "Exhale", "Inhale", null, null, "Soft elbows, wide arc, squeeze the chest at the top.", "Dumbbell Floor Flyes", "Chest", "3 sets x 12-15 reps", null, "Shoulders", 3, 15, 12, null, null, 1 },
                    { 4, null, 3, null, "Exhale", "Inhale", null, null, "Straight line head-to-heels, full range of motion.", "Push-ups", "Chest,Triceps", "To near-failure", null, "Shoulders,Core", 4, null, null, null, null, 1 },
                    { 5, null, 3, null, "Exhale", "Inhale", null, null, "Lead with the elbows, raise to shoulder height only.", "Lateral Raises", "Shoulders", "3 sets x 15 reps", null, "Traps", 5, 15, 15, null, null, 1 },
                    { 6, null, 3, null, "Exhale", "Inhale", null, null, "Keep elbows tucked, control the negative.", "Tricep Dumbbell Extension", "Triceps", "3 sets x 12-15 reps", null, null, 6, 15, 12, null, null, 1 },
                    { 7, null, 4, null, "Exhale", "Inhale", null, null, "Chest up, sit between the hips, knees track toes.", "Goblet Squat", "Quadriceps,Glutes", "4 sets x 12-15 reps", null, "Core,Hamstrings", 1, 15, 12, null, null, 2 },
                    { 8, null, 4, null, "Exhale", "Inhale", null, null, "Hinge at the hips, soft knees, feel the hamstrings.", "Dumbbell Romanian Deadlift", "Hamstrings,Glutes", "4 sets x 12 reps", null, "Lower Back", 2, 12, 12, null, null, 2 },
                    { 9, null, 3, null, "Exhale", "Inhale", null, null, "Long stride, back knee toward the floor, torso tall.", "Walking Lunges", "Quadriceps,Glutes", "3 sets x 12 reps/leg", null, "Hamstrings,Calves", 3, 12, 12, null, null, 2 },
                    { 10, null, 3, null, "Exhale", "Inhale", null, null, "Full stretch at the bottom, pause at the top.", "Standing Calf Raise", "Calves", "3 sets x 20 reps", null, null, 4, 20, 20, null, null, 2 },
                    { 11, null, 3, null, null, null, "Breathe slow and steady — don't hold your breath.", null, "Squeeze glutes and abs, no sagging hips.", "Plank Hold", "Core", "45-60 sec", null, "Shoulders,Glutes", 5, null, null, null, null, 2 },
                    { 12, null, 3, null, "Exhale", "Inhale", null, null, "Press low back down, lower legs slowly.", "Lying Leg Raises", "Core", "3 sets x 15 reps", null, "Hip Flexors", 6, 15, 15, null, null, 2 },
                    { 13, null, 4, null, "Exhale", "Inhale", null, null, "Flat back, drive elbow to hip, squeeze the lat.", "Single-Arm Dumbbell Row", "Back,Lats", "4 sets x 10-12 reps/side", null, "Biceps", 1, 12, 10, null, null, 3 },
                    { 14, null, 3, null, "Exhale", "Inhale", null, null, "Wide feet for stability, minimize hip rotation.", "Renegade Row", "Back,Core", "3 sets x 10 reps/side", null, "Biceps,Shoulders", 2, 10, 10, null, null, 3 },
                    { 15, null, 3, null, "Exhale", "Inhale", null, null, "Elbows pinned, no swinging, full squeeze.", "Bicep Curl", "Biceps", "3 sets x 12-15 reps", null, "Forearms", 3, 15, 12, null, null, 3 },
                    { 16, null, 3, null, "Exhale", "Inhale", null, null, "Neutral grip, control both directions.", "Hammer Curl", "Biceps,Forearms", "3 sets x 12-15 reps", null, null, 4, 15, 12, null, null, 3 },
                    { 17, null, 3, null, "Exhale", "Inhale", null, null, "Hinge forward, raise wide, lead with pinkies.", "Rear Delt Flye", "Rear Delts", "3 sets x 15 reps", null, "Traps,Back", 5, 15, 15, null, null, 3 },
                    { 18, null, 3, null, "Exhale", "Inhale", null, null, "Drive elbows down and back, chest to the edge.", "Towel Row Under Table (or Pull-ups)", "Back,Lats", "To near-failure", null, "Biceps", 6, null, null, null, null, 3 },
                    { 19, null, 4, null, "Exhale", "Inhale", null, null, "Squat, then drive through into the press in one motion.", "Dumbbell Thruster", "Quadriceps,Shoulders", "4 sets x 10-12 reps", null, "Glutes,Triceps,Core", 1, 12, 10, null, null, 4 },
                    { 20, null, 3, null, "Exhale", "Inhale", null, null, "Neutral spine, push the floor away, lock out tall.", "Dumbbell Deadlift", "Hamstrings,Glutes", "3 sets x 12 reps", null, "Back,Core", 2, 12, 12, null, null, 4 },
                    { 21, null, 3, null, "Exhale", "Inhale", null, null, "Straight line head-to-heels, full range of motion.", "Push-ups", "Chest,Triceps", "To near-failure", null, "Shoulders,Core", 3, null, null, null, null, 4 },
                    { 22, null, 3, null, "Exhale", "Inhale", null, null, "Hinge to ~45 degrees, row to the lower ribs.", "Bent-Over Row", "Back,Lats", "3 sets x 12 reps", null, "Biceps,Rear Delts", 4, 12, 12, null, null, 4 },
                    { 23, null, 3, null, "Exhale", "Inhale", null, null, "Rotate from the trunk, touch each side.", "Russian Twist", "Core,Obliques", "3 sets x 20 reps", null, "Hip Flexors", 5, 20, 20, null, null, 4 },
                    { 24, null, 3, null, "Exhale", "Inhale", null, null, "Opposite elbow to knee, slow and controlled.", "Bicycle Crunch", "Core,Obliques", "3 sets x 20 reps", null, "Hip Flexors", 6, 20, 20, null, null, 4 }
                });
        }
    }
}
