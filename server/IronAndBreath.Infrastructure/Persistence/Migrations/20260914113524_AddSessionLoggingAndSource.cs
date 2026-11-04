using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IronAndBreath.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionLoggingAndSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionSetLogs_Exercises_ExerciseId",
                table: "SessionSetLogs");

            migrationBuilder.AlterColumn<int>(
                name: "WorkoutDayId",
                table: "WorkoutSessions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "WorkoutSessions",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerceivedDifficulty",
                table: "WorkoutSessions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "WorkoutSessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ExerciseId",
                table: "SessionSetLogs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "ExerciseName",
                table: "SessionSetLogs",
                type: "TEXT",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "SessionSetLogs",
                type: "TEXT",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rpe",
                table: "SessionSetLogs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionSetLogs_Exercises_ExerciseId",
                table: "SessionSetLogs",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionSetLogs_Exercises_ExerciseId",
                table: "SessionSetLogs");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "PerceivedDifficulty",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "ExerciseName",
                table: "SessionSetLogs");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "SessionSetLogs");

            migrationBuilder.DropColumn(
                name: "Rpe",
                table: "SessionSetLogs");

            migrationBuilder.AlterColumn<int>(
                name: "WorkoutDayId",
                table: "WorkoutSessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExerciseId",
                table: "SessionSetLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionSetLogs_Exercises_ExerciseId",
                table: "SessionSetLogs",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
