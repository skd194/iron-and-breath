using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IronAndBreath.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthAndUserScoping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "WorkoutSessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "WorkoutDays",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserProgramSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    GoogleSubject = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 2,
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 3,
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "WorkoutDays",
                keyColumn: "Id",
                keyValue: 4,
                column: "UserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_UserId_Date",
                table: "WorkoutSessions",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutDays_UserId",
                table: "WorkoutDays",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgramSettings_UserId",
                table: "UserProgramSettings",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleSubject",
                table: "Users",
                column: "GoogleSubject",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProgramSettings_Users_UserId",
                table: "UserProgramSettings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutDays_Users_UserId",
                table: "WorkoutDays",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessions_Users_UserId",
                table: "WorkoutSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProgramSettings_Users_UserId",
                table: "UserProgramSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutDays_Users_UserId",
                table: "WorkoutDays");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_Users_UserId",
                table: "WorkoutSessions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_UserId_Date",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutDays_UserId",
                table: "WorkoutDays");

            migrationBuilder.DropIndex(
                name: "IX_UserProgramSettings_UserId",
                table: "UserProgramSettings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WorkoutDays");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserProgramSettings");
        }
    }
}
