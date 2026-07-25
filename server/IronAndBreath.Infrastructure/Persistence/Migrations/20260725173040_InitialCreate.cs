using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IronAndBreath.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExerciseVideos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Attribution = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseVideos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramProgressionPhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PhaseNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    WeekStart = table.Column<int>(type: "INTEGER", nullable: false),
                    WeekEnd = table.Column<int>(type: "INTEGER", nullable: false),
                    RepsHintText = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SetDelta = table.Column<int>(type: "INTEGER", nullable: false),
                    RestWorkSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    RestBetweenSetsSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    RestBetweenExercisesSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramProgressionPhases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProgramSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProgramStartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DaysPerWeekTarget = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 4)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProgramSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Focus = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkoutDayId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    TargetRepsLow = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetRepsHigh = table.Column<int>(type: "INTEGER", nullable: true),
                    RepsDisplay = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Cue = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseSets = table.Column<int>(type: "INTEGER", nullable: false),
                    VideoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_ExerciseVideos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "ExerciseVideos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Exercises_WorkoutDays_WorkoutDayId",
                        column: x => x.WorkoutDayId,
                        principalTable: "WorkoutDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkoutDayId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PhaseNumberAtCompletion = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSessions_WorkoutDays_WorkoutDayId",
                        column: x => x.WorkoutDayId,
                        principalTable: "WorkoutDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionSetLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkoutSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                    SetNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    RepsCompleted = table.Column<int>(type: "INTEGER", nullable: true),
                    WeightKg = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionSetLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionSetLogs_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionSetLogs_WorkoutSessions_WorkoutSessionId",
                        column: x => x.WorkoutSessionId,
                        principalTable: "WorkoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ProgramProgressionPhases",
                columns: new[] { "Id", "Notes", "PhaseNumber", "RepsHintText", "RestBetweenExercisesSeconds", "RestBetweenSetsSeconds", "RestWorkSeconds", "SetDelta", "WeekEnd", "WeekStart" },
                values: new object[,]
                {
                    { 1, "Focus on slow, controlled tempo and full range of motion.", 1, "10-12 reps, slow controlled tempo", 120, 90, 40, 0, 4, 1 },
                    { 2, "Shorter rest. Encourage supersetting where possible.", 2, "15-20 reps, higher volume", 90, 60, 40, 0, 8, 5 },
                    { 3, "Pause reps for tension. Optional drop set on the last set of each exercise.", 3, "Add a set, pause reps, shortest rest", 75, 45, 45, 1, 12, 9 }
                });

            migrationBuilder.InsertData(
                table: "WorkoutDays",
                columns: new[] { "Id", "Focus", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Chest, shoulders, triceps", "Upper Body (Push)", 1 },
                    { 2, "Quads, hamstrings, glutes, core", "Lower Body + Core", 2 },
                    { 3, "Back, biceps, rear delts", "Upper Body (Pull)", 3 },
                    { 4, "Total body, conditioning, core", "Full Body (Metabolic)", 4 }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "BaseSets", "Cue", "Name", "RepsDisplay", "SortOrder", "TargetRepsHigh", "TargetRepsLow", "VideoId", "WorkoutDayId" },
                values: new object[,]
                {
                    { 1, 4, "Lower elbows to the floor, pause, press up.", "Dumbbell Floor Press", "4 sets x 10-12 reps", 1, 12, 10, null, 1 },
                    { 2, 3, "Brace core, press overhead without arching the back.", "Dumbbell Shoulder Press", "3 sets x 10-12 reps", 2, 12, 10, null, 1 },
                    { 3, 3, "Soft elbows, wide arc, squeeze the chest at the top.", "Dumbbell Floor Flyes", "3 sets x 12-15 reps", 3, 15, 12, null, 1 },
                    { 4, 3, "Straight line head-to-heels, full range of motion.", "Push-ups", "To near-failure", 4, null, null, null, 1 },
                    { 5, 3, "Lead with the elbows, raise to shoulder height only.", "Lateral Raises", "3 sets x 15 reps", 5, 15, 15, null, 1 },
                    { 6, 3, "Keep elbows tucked, control the negative.", "Tricep Dumbbell Extension", "3 sets x 12-15 reps", 6, 15, 12, null, 1 },
                    { 7, 4, "Chest up, sit between the hips, knees track toes.", "Goblet Squat", "4 sets x 12-15 reps", 1, 15, 12, null, 2 },
                    { 8, 4, "Hinge at the hips, soft knees, feel the hamstrings.", "Dumbbell Romanian Deadlift", "4 sets x 12 reps", 2, 12, 12, null, 2 },
                    { 9, 3, "Long stride, back knee toward the floor, torso tall.", "Walking Lunges", "3 sets x 12 reps/leg", 3, 12, 12, null, 2 },
                    { 10, 3, "Full stretch at the bottom, pause at the top.", "Standing Calf Raise", "3 sets x 20 reps", 4, 20, 20, null, 2 },
                    { 11, 3, "Squeeze glutes and abs, no sagging hips.", "Plank Hold", "45-60 sec", 5, null, null, null, 2 },
                    { 12, 3, "Press low back down, lower legs slowly.", "Lying Leg Raises", "3 sets x 15 reps", 6, 15, 15, null, 2 },
                    { 13, 4, "Flat back, drive elbow to hip, squeeze the lat.", "Single-Arm Dumbbell Row", "4 sets x 10-12 reps/side", 1, 12, 10, null, 3 },
                    { 14, 3, "Wide feet for stability, minimize hip rotation.", "Renegade Row", "3 sets x 10 reps/side", 2, 10, 10, null, 3 },
                    { 15, 3, "Elbows pinned, no swinging, full squeeze.", "Bicep Curl", "3 sets x 12-15 reps", 3, 15, 12, null, 3 },
                    { 16, 3, "Neutral grip, control both directions.", "Hammer Curl", "3 sets x 12-15 reps", 4, 15, 12, null, 3 },
                    { 17, 3, "Hinge forward, raise wide, lead with pinkies.", "Rear Delt Flye", "3 sets x 15 reps", 5, 15, 15, null, 3 },
                    { 18, 3, "Drive elbows down and back, chest to the edge.", "Towel Row Under Table (or Pull-ups)", "To near-failure", 6, null, null, null, 3 },
                    { 19, 4, "Squat, then drive through into the press in one motion.", "Dumbbell Thruster", "4 sets x 10-12 reps", 1, 12, 10, null, 4 },
                    { 20, 3, "Neutral spine, push the floor away, lock out tall.", "Dumbbell Deadlift", "3 sets x 12 reps", 2, 12, 12, null, 4 },
                    { 21, 3, "Straight line head-to-heels, full range of motion.", "Push-ups", "To near-failure", 3, null, null, null, 4 },
                    { 22, 3, "Hinge to ~45 degrees, row to the lower ribs.", "Bent-Over Row", "3 sets x 12 reps", 4, 12, 12, null, 4 },
                    { 23, 3, "Rotate from the trunk, touch each side.", "Russian Twist", "3 sets x 20 reps", 5, 20, 20, null, 4 },
                    { 24, 3, "Opposite elbow to knee, slow and controlled.", "Bicycle Crunch", "3 sets x 20 reps", 6, 20, 20, null, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_VideoId",
                table: "Exercises",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_WorkoutDayId_SortOrder",
                table: "Exercises",
                columns: new[] { "WorkoutDayId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramProgressionPhases_PhaseNumber",
                table: "ProgramProgressionPhases",
                column: "PhaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionSetLogs_ExerciseId",
                table: "SessionSetLogs",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSetLogs_WorkoutSessionId",
                table: "SessionSetLogs",
                column: "WorkoutSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutDays_SortOrder",
                table: "WorkoutDays",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_Date",
                table: "WorkoutSessions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutDayId",
                table: "WorkoutSessions",
                column: "WorkoutDayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramProgressionPhases");

            migrationBuilder.DropTable(
                name: "SessionSetLogs");

            migrationBuilder.DropTable(
                name: "UserProgramSettings");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "WorkoutSessions");

            migrationBuilder.DropTable(
                name: "ExerciseVideos");

            migrationBuilder.DropTable(
                name: "WorkoutDays");
        }
    }
}
