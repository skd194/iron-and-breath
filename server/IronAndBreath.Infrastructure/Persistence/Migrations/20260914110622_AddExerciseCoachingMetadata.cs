using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IronAndBreath.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseCoachingMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnimationRef",
                table: "Exercises",
                type: "TEXT",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "Exercises",
                type: "TEXT",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BreathingConcentric",
                table: "Exercises",
                type: "TEXT",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BreathingEccentric",
                table: "Exercises",
                type: "TEXT",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BreathingNotes",
                table: "Exercises",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommonMistakes",
                table: "Exercises",
                type: "TEXT",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryMuscles",
                table: "Exercises",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SafetyTips",
                table: "Exercises",
                type: "TEXT",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryMuscles",
                table: "Exercises",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tempo",
                table: "Exercises",
                type: "TEXT",
                maxLength: 60,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Chest,Triceps", null, "Shoulders", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Shoulders,Triceps", null, "Upper Chest", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Chest", null, "Shoulders", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Chest,Triceps", null, "Shoulders,Core", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Shoulders", null, "Traps", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Triceps", null, null, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Quadriceps,Glutes", null, "Core,Hamstrings", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Hamstrings,Glutes", null, "Lower Back", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Quadriceps,Glutes", null, "Hamstrings,Calves", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Calves", null, null, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, null, null, "Breathe slow and steady — don't hold your breath.", null, "Core", null, "Shoulders,Glutes", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Core", null, "Hip Flexors", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Back,Lats", null, "Biceps", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Back,Core", null, "Biceps,Shoulders", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Biceps", null, "Forearms", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Biceps,Forearms", null, null, null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Rear Delts", null, "Traps,Back", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Back,Lats", null, "Biceps", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Quadriceps,Shoulders", null, "Glutes,Triceps,Core", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Hamstrings,Glutes", null, "Back,Core", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Chest,Triceps", null, "Shoulders,Core", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Back,Lats", null, "Biceps,Rear Delts", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Core,Obliques", null, "Hip Flexors", null });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "AnimationRef", "Benefits", "BreathingConcentric", "BreathingEccentric", "BreathingNotes", "CommonMistakes", "PrimaryMuscles", "SafetyTips", "SecondaryMuscles", "Tempo" },
                values: new object[] { null, null, "Exhale", "Inhale", null, null, "Core,Obliques", null, "Hip Flexors", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimationRef",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "BreathingConcentric",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "BreathingEccentric",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "BreathingNotes",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "CommonMistakes",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "PrimaryMuscles",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "SafetyTips",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "SecondaryMuscles",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "Tempo",
                table: "Exercises");
        }
    }
}
