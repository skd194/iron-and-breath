using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IronAndBreath.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdjustTemposAndPullupBar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EquipmentItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "Name", "Specs" },
                values: new object[] { 7, "Doorway Pull-up Bar", "Fits in a doorway — pull-ups, chin-ups and hanging leg raises." });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9001,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9002,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9003,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9004,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9005,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9006,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9007,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9008,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9009,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9010,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "pull-ups", "The best bodyweight back and lat builder — uses your pull-up bar.", "Kipping/swinging, half reps, not controlling the descent.", "Dead hang from the doorway bar, pull your chin over, lower under control.", "Pull-ups", "Back,Lats", "3 sets x 6-10 reps (or to near-failure)", "Full controlled range; use a band or slow negatives if you can't do full reps yet.", "Biceps,Rear Delts", 10, 6, "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9011,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9012,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9013,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9015,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9016,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9017,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9018,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9019,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s · 1s pause", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9021,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "hanging-leg-raise", "A strong lower-ab and grip builder using your pull-up bar.", "Swinging the legs up with momentum.", "Hang from the doorway bar, raise the legs with control, don't swing.", "Hanging Leg Raise", "3 sets x 10-15 reps", "Keep it controlled; stop if the lower back rounds painfully.", "Hip Flexors,Forearms", 10, "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9023,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9024,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9025,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9026,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 2s · Down 3s", 2, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EquipmentItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "Name", "Specs" },
                values: new object[] { 5, "Resistance Ropes", "Attach to the bench for face pulls and presses." });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9001,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 1s · Down 3s", 1 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9002,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9003,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 1s · Down 3s", 1 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9004,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9005,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9006,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 1s · Down 3s", 1 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9007,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9008,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9009,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9010,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "PrimaryMuscles", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "TargetRepsHigh", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "resistance-rope-face-pull", "Bulletproofs the shoulders and improves posture.", "Using too much weight and turning it into a row.", "Pull the ropes to your forehead, elbows high, squeeze the rear delts.", "Resistance-Rope Face Pull", "Rear Delts,Traps", "3 sets x 15-20 reps", "Keep it light and controlled; this is a health movement.", "Back", 20, 15, "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9011,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 1s · Down 3s", 1 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9012,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9013,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9015,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 1s · Down 3s", 1 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9016,
                columns: new[] { "Tempo", "TempoConcentricSeconds" },
                values: new object[] { "Up 1s · Down 3s", 1 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9017,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9018,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9019,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s · 1s pause", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9021,
                columns: new[] { "AnimationRef", "Benefits", "CommonMistakes", "Cue", "Name", "RepsDisplay", "SafetyTips", "SecondaryMuscles", "TargetRepsLow", "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "lying-leg-raise", "Targets the lower abs.", "Arching the lower back off the floor.", "Press the lower back down, raise the legs, lower slowly.", "Lying Leg Raise", "3 sets x 15 reps", "Keep the lower back pinned down the whole set.", "Hip Flexors", 15, "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9023,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9024,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9025,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9026,
                columns: new[] { "Tempo", "TempoConcentricSeconds", "TempoEccentricSeconds" },
                values: new object[] { "Up 1s · Down 2s", 1, 2 });
        }
    }
}
