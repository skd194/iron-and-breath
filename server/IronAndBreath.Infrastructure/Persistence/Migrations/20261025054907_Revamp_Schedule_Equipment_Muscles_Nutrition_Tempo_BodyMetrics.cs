using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IronAndBreath.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Revamp_Schedule_Equipment_Muscles_Nutrition_Tempo_BodyMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "RepsDisplay", "SafetyTips", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "flat-barbell-bench-press", "The foundational horizontal press for chest and triceps mass.", "Flaring elbows to 90°, bouncing the bar off the chest.", "Shoulder blades pinched, bar to mid-chest, drive up and slightly back.", "Flat Barbell Bench Press", "4 sets x 8-12 reps", "Keep wrists stacked over elbows; use a spotter or safety at light-but-heavy loads.", 8, "Up 1s · Down 3s", 1, 3, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "incline-dumbbell-press", "Targets the upper chest the flat press under-trains.", "Setting the bench too steep (turns it into a shoulder press).", "Bench at 30°, press the dumbbells up and together without clanging.", "Incline Dumbbell Press", "Upper Chest,Shoulders", "Control the dumbbells at the bottom; don't let shoulders roll forward.", "Triceps", "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "SafetyTips", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "incline-dumbbell-flye", "Isolates the chest through a long stretch for extra hypertrophy.", "Bending the elbows into a press, going too heavy.", "Soft elbows, wide arc, feel the stretch, squeeze at the top.", "Incline Dumbbell Flye", "Keep a slight elbow bend throughout to protect the shoulder joint.", "Up 1s · Down 3s", 1, 3, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "seated-dumbbell-shoulder-press", "Builds overhead pressing strength and delt size.", "Over-arching the back, pressing with a forward lean.", "Brace the core, press overhead without arching the lower back.", "Seated Dumbbell Shoulder Press", "Shoulders,Triceps", "3 sets x 10-12 reps", "Keep ribs down; stop the range where shoulders feel pinch-free.", "Upper Chest", 12, 10, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "RepsDisplay", "SafetyTips", "TargetRepsHigh", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "lateral-raise", "Widens the shoulders by isolating the side delts.", "Swinging the weight, shrugging the traps up.", "Lead with the elbows, raise to shoulder height only, control down.", "Lateral Raise", "3 sets x 15-20 reps", "Go light; momentum here strains the shoulder and neck.", 20, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "SafetyTips", "SecondaryMuscles", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "overhead-triceps-extension", "Hits the long head of the triceps through a deep stretch.", "Flaring the elbows out, using the shoulders to lift.", "Elbows tucked and pointing forward, full stretch behind the head.", "Overhead Triceps Extension", "Lower under control; don't let the weight yank the elbow open.", "Shoulders", "Up 1s · Down 3s", 1, 3, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds", "WorkoutDayId" },
                values: new object[] { "push-ups", 2, "A bodyweight finisher to fully fatigue the chest and triceps.", "Sagging hips, half-range reps.", "Straight line head-to-heels, full range, chest to the floor.", "Push-ups", "Chest,Triceps", "2 sets to near-failure", "Keep the core braced to protect the lower back.", "Shoulders,Core", 7, null, null, "Up 1s · Down 2s", 1, 2, null, 1 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "barbell-bent-over-row", "The primary mass builder for the whole back.", "Rounding the back, jerking with the hips.", "Hinge to ~45°, flat back, pull the bar to the lower ribs.", "Barbell Bent-Over Row", "Back,Lats", "4 sets x 8-12 reps", "Keep a neutral spine; brace hard to protect the lower back.", "Biceps,Rear Delts", 1, 8, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "one-arm-dumbbell-row", "Unilateral row that fixes left/right imbalances.", "Twisting the torso to heave the weight up.", "Hand and knee on the bench, drive the elbow to the hip, squeeze the lat.", "One-Arm Dumbbell Row", "Lats,Back", "3 sets x 10-12 reps/side", "Keep the spine neutral and hips square.", "Biceps", 2, 10, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "resistance-rope-face-pull", "Bulletproofs the shoulders and improves posture.", "Using too much weight and turning it into a row.", "Pull the ropes to your forehead, elbows high, squeeze the rear delts.", "Resistance-Rope Face Pull", "Rear Delts,Traps", "3 sets x 15-20 reps", "Keep it light and controlled; this is a health movement.", "Back", 3, 15, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "barbell-curl", "The classic mass builder for the biceps.", "Exhale", "Inhale", null, "Swinging the torso, using the shoulders to lift.", "Elbows pinned to the sides, curl the curl-rod, no swinging.", "Barbell Curl", "Biceps", "3 sets x 10-12 reps", "Keep wrists neutral; the EZ bar is easier on them.", "Forearms", 4, 12, 10, "Up 1s · Down 3s", 1, 3, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "hammer-curl", "Builds the brachialis for thicker arms and strong forearms.", "Rushing the lowering phase.", "Neutral grip (palms facing in), control both directions.", "Hammer Curl", "Biceps,Forearms", "3 sets x 12-15 reps", "No swinging; keep the elbows still.", null, 5, 12, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds", "WorkoutDayId" },
                values: new object[] { "rear-delt-flye", 3, "Balances the shoulders and improves posture.", "Using the lower back to swing the weight.", "Hinge forward, raise the dumbbells wide, lead with the pinkies.", "Rear-Delt Flye", "Rear Delts", "3 sets x 15-20 reps", "Light weight, slow tempo; protect the lower back by bracing.", "Traps,Back", 6, 20, 15, "Up 1s · Down 2s", 1, 2, null, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds", "WorkoutDayId" },
                values: new object[] { "superman-back-extension-hold", "Strengthens the spinal erectors to relieve back tightness.", null, null, "Breathe slow and steady — don't hold your breath.", "Cranking the neck back.", "Lift chest and thighs off the floor, squeeze the glutes, breathe.", "Superman / Back Extension Hold", "Lower Back,Glutes", "3 sets x 30-45 sec", "Keep the neck neutral; stop if it pinches the lower back.", "Core", 7, null, null, null, null, null, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "barbell-back-squat", 4, "The king of lower-body mass and strength.", "Knees caving in, heels lifting, rounding the back.", "Chest up, sit between the hips, knees track the toes, drive up.", "Barbell Back Squat", "Quadriceps,Glutes", "4 sets x 8-12 reps", "Brace the core; keep depth where your back stays neutral.", "Hamstrings,Core", 1, 12, 8, "Up 1s · Down 3s", 1, 3, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "romanian-deadlift", 4, "Builds the hamstrings and glutes and protects the back.", "Rounding the back, squatting instead of hinging.", "Hinge at the hips, soft knees, bar close, feel the hamstring stretch.", "Romanian Deadlift", "Hamstrings,Glutes", "4 sets x 10-12 reps", "Keep the bar against the legs and the spine neutral throughout.", "Lower Back", 2, 12, 10, "Up 1s · Down 3s", 1, 3, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "bulgarian-split-squat", "Unilateral leg builder that also trains balance.", "Leaning too far forward, letting the front knee cave.", "Rear foot on the bench, drop straight down, drive through the front heel.", "Bulgarian Split Squat", "Quadriceps,Glutes", "3 sets x 10-12 reps/leg", "Keep the front knee tracking over the foot.", "Hamstrings", 3, 12, 10, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "walking-lunge", "Trains the legs through a long, athletic range.", "Short steps that overload the knee.", "Long stride, back knee toward the floor, torso tall.", "Walking Lunge", "Quadriceps,Glutes", "3 sets x 12 reps/leg", "Step long enough that the front shin stays near vertical.", "Hamstrings,Calves", 4, 12, 12, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds", "WorkoutDayId" },
                values: new object[] { "standing-calf-raise", "Builds the calves, which need high reps and a strong stretch.", "Bouncing through a short range.", "Full stretch at the bottom, pause and squeeze at the top.", "Standing Calf Raise", "Calves", "4 sets x 20 reps", "Control the bottom stretch; don't ballistically drop.", null, 5, 20, 20, "Up 1s · Down 2s · 1s pause", 1, 2, 1, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds", "WorkoutDayId" },
                values: new object[] { "plank-hold", "Builds deep core stability that protects the spine.", null, null, "Breathe slow and steady — don't hold your breath.", "Hips sagging or piking up.", "Squeeze the glutes and abs, straight line, no sagging hips.", "Plank Hold", "Core", "3 sets x 45-60 sec", "Keep a neutral neck and stop before the lower back sags.", "Shoulders,Glutes", 6, null, null, null, null, null, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds", "WorkoutDayId" },
                values: new object[] { "lying-leg-raise", "Targets the lower abs.", "Arching the lower back off the floor.", "Press the lower back down, raise the legs, lower slowly.", "Lying Leg Raise", "Core", "3 sets x 15 reps", "Keep the lower back pinned down the whole set.", "Hip Flexors", 7, 15, 15, "Up 1s · Down 2s", 1, 2, null, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "skipping-warm-up-intervals", 1, "Raises the heart rate and primes the whole body.", null, null, "Breathe rhythmically with the rope.", "Jumping too high and landing hard.", "Light bounces on the balls of the feet, relaxed shoulders.", "Skipping — Warm-up Intervals", "Calves,Shoulders", "3-5 min easy pace", "Land softly through the balls of the feet to spare the knees.", "Core", 1, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "dumbbell-thruster", 4, "A full-body compound that burns a lot in little time.", "Pressing before the legs finish driving.", "Squat, then drive up and press overhead in one fluid motion.", "Dumbbell Thruster", "Quadriceps,Shoulders", "4 sets x 10-12 reps", "Keep the core braced; don't arch the back at lockout.", "Glutes,Triceps,Core", 2, 12, 10, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds" },
                values: new object[] { "dumbbell-deadlift", "Trains the posterior chain safely with lighter load.", "Rounding the back, jerking off the floor.", "Neutral spine, push the floor away, lock out tall.", "Dumbbell Deadlift", "Hamstrings,Glutes", "3 sets x 12 reps", "Hinge, don't squat; keep the spine neutral.", "Back,Core", 3, 12, 12, "Up 1s · Down 2s", 1, 2, null });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "AnimationRef", "BaseSets", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds", "VideoId", "WorkoutDayId" },
                values: new object[,]
                {
                    { 25, "renegade-row", 3, "Combines a row with a hard anti-rotation core challenge.", "Exhale", "Inhale", null, "Twisting the hips toward the working arm.", "Wide feet for stability, row one dumbbell, minimise hip rotation.", "Renegade Row", "Back,Core", "3 sets x 10 reps/side", "Set feet wide and keep the hips level.", "Biceps,Shoulders", 4, 10, 10, "Up 1s · Down 2s", 1, 2, null, null, 4 },
                    { 26, "push-ups", 3, "Bodyweight pressing volume for conditioning.", "Exhale", "Inhale", null, "Sagging hips, partial reps.", "Straight line head-to-heels, full range of motion.", "Push-ups", "Chest,Triceps", "3 sets to near-failure", "Brace the core to protect the lower back.", "Shoulders,Core", 5, null, null, "Up 1s · Down 2s", 1, 2, null, null, 4 },
                    { 27, "russian-twist", 3, "Builds rotational core strength and the obliques.", "Exhale", "Inhale", null, "Moving only the arms, not the torso.", "Rotate from the trunk, touch each side, feet hovering if able.", "Russian Twist", "Core,Obliques", "3 sets x 20 reps", "Rotate from the ribcage, not by yanking the lower back.", "Hip Flexors", 6, 20, 20, "Up 1s · Down 1s", 1, 1, null, null, 4 },
                    { 28, "skipping-finisher", 1, "Caps the session with a conditioning burst.", null, null, "Breathe rhythmically with the rope.", "Tensing the shoulders.", "Steady rhythm, pick up the pace for the last 30 seconds.", "Skipping — Finisher", "Calves,Shoulders", "2-3 min, build pace", "Keep landings soft; stop if the calves cramp.", "Core", 7, null, null, null, null, null, null, null, 4 }
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

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Focus", "Name", "ScheduledDayOfWeek" },
                values: new object[] { "Horizontal + overhead pressing", "Push · Chest / Shoulders / Triceps", 6 });

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Focus", "Name", "ScheduledDayOfWeek" },
                values: new object[] { "Rows, pulls, curls, back health", "Pull · Back / Biceps / Rear Delts", 0 });

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Focus", "Name", "ScheduledDayOfWeek" },
                values: new object[] { "Squat, hinge, lunge, trunk", "Legs + Core", 1 });

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Focus", "Name", "ScheduledDayOfWeek" },
                values: new object[] { "Compound circuit with skipping", "Full-Body Metabolic + Cardio", 3 });

            migrationBuilder.InsertData(
                table: "WorkoutDays",
                columns: new[] { "Id", "Focus", "Name", "ScheduledDayOfWeek", "SortOrder", "UserId" },
                values: new object[] { 5, "Full-body stretch, back-focused recovery", "Mobility & Yoga", 4, 5, null });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "AnimationRef", "BaseSets", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds", "TempoPauseSeconds", "VideoId", "WorkoutDayId" },
                values: new object[,]
                {
                    { 29, "cat-cow", 1, "Mobilises the whole spine and eases back stiffness.", null, null, "Inhale to arch, exhale to round.", "Rushing; forcing the range.", "Alternate arching and rounding the spine with the breath.", "Cat-Cow", "Spinal Erectors,Core", "60 sec, slow flow", "Move only as far as comfortable.", "Lower Back", 1, null, null, null, null, null, null, null, 5 },
                    { 30, "child-s-pose", 1, "Gently stretches the back and lats and calms the nervous system.", null, null, "Breathe deep into the back of the ribs.", "Holding tension in the shoulders.", "Hips back to the heels, arms long, let the back decompress.", "Child's Pose", "Lats,Lower Back", "60 sec hold", "Widen the knees if the belly or hips feel cramped.", "Shoulders", 2, null, null, null, null, null, null, null, 5 },
                    { 31, "downward-dog", 1, "Stretches the whole back line — calves, hamstrings, spine.", null, null, "Breathe slow and steady.", "Rounding the back to force the heels down.", "Hips high, heels reaching down, spine long.", "Downward Dog", "Hamstrings,Calves", "45 sec hold", "Bend the knees as needed to keep the spine long.", "Shoulders,Back", 3, null, null, null, null, null, null, null, 5 },
                    { 32, "cobra-sphinx", 1, "Counters hunching and gently extends the lower back.", null, null, "Breathe into the chest.", "Cranking the neck or over-extending.", "Lift the chest, elbows under shoulders, shoulders down away from ears.", "Cobra / Sphinx", "Lower Back,Core", "45 sec hold", "Keep the lift gentle; no sharp lower-back pinch.", "Chest", 4, null, null, null, null, null, null, null, 5 },
                    { 33, "thread-the-needle", 1, "Opens the upper back and shoulders.", null, null, "Breathe slow and steady.", "Collapsing the supporting shoulder.", "From all fours, thread one arm under the other, rest on the shoulder.", "Thread-the-Needle", "Upper Back,Rear Delts", "45 sec/side", "Keep weight evenly distributed; move slowly.", "Shoulders", 5, null, null, null, null, null, null, null, 5 },
                    { 34, "supine-spinal-twist", 1, "Releases the lower back and obliques — great for tightness.", null, null, "Exhale as you sink deeper.", "Forcing the knees down and lifting the shoulder.", "On your back, drop the knees to one side, arms wide, gaze opposite.", "Supine Spinal Twist", "Lower Back,Obliques", "45 sec/side", "Keep both shoulders on the floor; let gravity do the work.", "Glutes,Spinal Erectors", 6, null, null, null, null, null, null, null, 5 },
                    { 35, "seated-forward-fold", 1, "Lengthens the hamstrings and the whole back line.", null, null, "Exhale to fold a little deeper.", "Rounding hard from the upper back to reach further.", "Hinge from the hips, long spine, reach toward the feet.", "Seated Forward Fold", "Hamstrings,Lower Back", "60 sec hold", "Bend the knees slightly; lead with the chest, not the head.", "Calves", 7, null, null, null, null, null, null, null, 5 },
                    { 36, "pigeon-pose", 1, "Opens tight hips and glutes that pull on the lower back.", null, null, "Breathe slow and steady.", "Collapsing onto one hip.", "Front shin angled, back leg long, fold over the front leg.", "Pigeon Pose", "Glutes,Hip Flexors", "45 sec/side", "Prop a cushion under the hip to keep it level.", "Lower Back", 8, null, null, null, null, null, null, null, 5 },
                    { 37, "standing-hamstring-hip-flexor-stretch", 1, "Balances the front and back of the hips for a neutral pelvis.", null, null, "Breathe slow and steady.", "Bouncing into the stretch.", "Stretch one hamstring, then lunge to open the opposite hip flexor.", "Standing Hamstring + Hip-Flexor Stretch", "Hamstrings,Hip Flexors", "45 sec/side", "Ease in gradually; never bounce.", "Quadriceps", 9, null, null, null, null, null, null, null, 5 },
                    { 38, "neck-shoulder-release", 1, "Releases desk-and-training tension in the neck and traps.", null, null, "Breathe slow and steady.", "Pulling the head hard with the hand.", "Gently tilt the ear to the shoulder, roll the shoulders back and down.", "Neck & Shoulder Release", "Traps,Neck", "45 sec hold", "Use only light pressure; the neck is delicate.", "Shoulders", 10, null, null, null, null, null, null, null, 5 }
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
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 5);

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

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "RepsDisplay", "SafetyTips", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, "Lower elbows to the floor, pause, press up.", "Dumbbell Floor Press", "4 sets x 10-12 reps", null, 10, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, null, "Brace core, press overhead without arching the back.", "Dumbbell Shoulder Press", "Shoulders,Triceps", null, "Upper Chest", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "SafetyTips", "Tempo" },
                values: new object[] { null, null, null, "Soft elbows, wide arc, squeeze the chest at the top.", "Dumbbell Floor Flyes", null, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "TargetRepsHigh", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, "Straight line head-to-heels, full range of motion.", "Push-ups", "Chest,Triceps", "To near-failure", null, "Shoulders,Core", null, null, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "RepsDisplay", "SafetyTips", "TargetRepsHigh", "Tempo" },
                values: new object[] { null, null, null, "Lead with the elbows, raise to shoulder height only.", "Lateral Raises", "3 sets x 15 reps", null, 15, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, null, "Keep elbows tucked, control the negative.", "Tricep Dumbbell Extension", null, null, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "WorkoutDayId" },
                values: new object[] { null, 4, null, null, "Chest up, sit between the hips, knees track toes.", "Goblet Squat", "Quadriceps,Glutes", "4 sets x 12-15 reps", null, "Core,Hamstrings", 1, 15, 12, null, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, "Hinge at the hips, soft knees, feel the hamstrings.", "Dumbbell Romanian Deadlift", "Hamstrings,Glutes", "4 sets x 12 reps", null, "Lower Back", 2, 12, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, "Long stride, back knee toward the floor, torso tall.", "Walking Lunges", "Quadriceps,Glutes", "3 sets x 12 reps/leg", null, "Hamstrings,Calves", 3, 12, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, "Full stretch at the bottom, pause at the top.", "Standing Calf Raise", "Calves", "3 sets x 20 reps", null, null, 4, 20, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, null, "Breathe slow and steady — don't hold your breath.", null, "Squeeze glutes and abs, no sagging hips.", "Plank Hold", "Core", "45-60 sec", null, "Shoulders,Glutes", 5, null, null, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, "Press low back down, lower legs slowly.", "Lying Leg Raises", "Core", "3 sets x 15 reps", null, "Hip Flexors", 6, 15, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "WorkoutDayId" },
                values: new object[] { null, 4, null, null, "Flat back, drive elbow to hip, squeeze the lat.", "Single-Arm Dumbbell Row", "Back,Lats", "4 sets x 10-12 reps/side", null, "Biceps", 1, 12, 10, null, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "WorkoutDayId" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Wide feet for stability, minimize hip rotation.", "Renegade Row", "Back,Core", "3 sets x 10 reps/side", null, "Biceps,Shoulders", 2, 10, 10, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo" },
                values: new object[] { null, 3, null, null, "Elbows pinned, no swinging, full squeeze.", "Bicep Curl", "Biceps", "3 sets x 12-15 reps", null, "Forearms", 3, 15, 12, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo" },
                values: new object[] { null, 3, null, null, "Neutral grip, control both directions.", "Hammer Curl", "Biceps,Forearms", "3 sets x 12-15 reps", null, null, 4, 15, 12, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, "Hinge forward, raise wide, lead with pinkies.", "Rear Delt Flye", "Rear Delts", "3 sets x 15 reps", null, "Traps,Back", 5, 15, 15, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, "Drive elbows down and back, chest to the edge.", "Towel Row Under Table (or Pull-ups)", "Back,Lats", "To near-failure", null, "Biceps", 6, null, null, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "WorkoutDayId" },
                values: new object[] { null, null, null, "Squat, then drive through into the press in one motion.", "Dumbbell Thruster", "Quadriceps,Shoulders", "4 sets x 10-12 reps", null, "Glutes,Triceps,Core", 1, 12, 10, null, 4 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "WorkoutDayId" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Neutral spine, push the floor away, lock out tall.", "Dumbbell Deadlift", "Hamstrings,Glutes", "3 sets x 12 reps", null, "Back,Core", 2, 12, 12, 4 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo", "WorkoutDayId" },
                values: new object[] { null, null, null, "Straight line head-to-heels, full range of motion.", "Push-ups", "Chest,Triceps", "To near-failure", null, "Shoulders,Core", 3, null, null, null, 4 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow" },
                values: new object[] { null, 3, null, "Exhale", "Inhale", null, null, "Hinge to ~45 degrees, row to the lower ribs.", "Bent-Over Row", "Back,Lats", "3 sets x 12 reps", null, "Biceps,Rear Delts", 4, 12, 12 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "AnimationRef", "BaseSets", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo" },
                values: new object[] { null, 3, null, null, "Rotate from the trunk, touch each side.", "Russian Twist", "Core,Obliques", "3 sets x 20 reps", null, "Hip Flexors", 5, 20, 20, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "Tempo" },
                values: new object[] { null, null, null, "Opposite elbow to knee, slow and controlled.", "Bicycle Crunch", "Core,Obliques", "3 sets x 20 reps", null, "Hip Flexors", 6, 20, 20, null });

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Focus", "Name" },
                values: new object[] { "Chest, shoulders, triceps", "Upper Body (Push)" });

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Focus", "Name" },
                values: new object[] { "Quads, hamstrings, glutes, core", "Lower Body + Core" });

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Focus", "Name" },
                values: new object[] { "Back, biceps, rear delts", "Upper Body (Pull)" });

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Focus", "Name" },
                values: new object[] { "Total body, conditioning, core", "Full Body (Metabolic)" });
        }
    }
}
