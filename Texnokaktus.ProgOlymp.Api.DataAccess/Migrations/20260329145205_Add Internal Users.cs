using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinalStageParticipation_InternalUserId",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreliminaryStageParticipation_InternalUserId",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InternalUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "varchar(900)", unicode: false, nullable: false),
                    Password = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    IsDeprecated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalUsers", x => x.Id);
                    table.UniqueConstraint("AK_InternalUsers_Login", x => x.Login);
                });

            migrationBuilder.InsertData(
                table: "InternalUsers",
                columns: new[] { "Id", "IsDeprecated", "Login", "Password" },
                values: new object[,]
                {
                    { 1, false, "progolymp26-1", "azjcLgyXJv" },
                    { 2, false, "progolymp26-2", "b8z2jntRwz" },
                    { 3, false, "progolymp26-3", "2NMAbpzgQR" },
                    { 4, false, "progolymp26-4", "MEn8Hqn4UQ" },
                    { 5, false, "progolymp26-5", "T2tcR5HVbf" },
                    { 6, false, "progolymp26-6", "z2zXnhvFC8" },
                    { 7, false, "progolymp26-7", "EfRfadqAH2" },
                    { 8, false, "progolymp26-8", "wGsuGTMub7" },
                    { 9, false, "progolymp26-9", "3fYLTvd6xV" },
                    { 10, false, "progolymp26-10", "s7LLRugyrx" },
                    { 11, false, "progolymp26-11", "bpBevLM8tN" },
                    { 12, false, "progolymp26-12", "yc7zwrkBF5" },
                    { 13, false, "progolymp26-13", "p9zV3Kbe65" },
                    { 14, false, "progolymp26-14", "vsuFNyFNXQ" },
                    { 15, false, "progolymp26-15", "WmYNXYArw6" },
                    { 16, false, "progolymp26-16", "Q8Me5d6fms" },
                    { 17, false, "progolymp26-17", "Td7wnGbHqW" },
                    { 18, false, "progolymp26-18", "r2neJRTrVa" },
                    { 19, false, "progolymp26-19", "qJJ7j8Dc5k" },
                    { 20, false, "progolymp26-20", "3DAJaepkyH" },
                    { 21, false, "progolymp26-21", "hGpmUuwtua" },
                    { 22, false, "progolymp26-22", "AbmFcMjVJw" },
                    { 23, false, "progolymp26-23", "Qg9FKcekgm" },
                    { 24, false, "progolymp26-24", "JD37UJxhba" },
                    { 25, false, "progolymp26-25", "XWp3Vcmh7t" },
                    { 26, false, "progolymp26-26", "aRFsQVbJK5" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternalUsers");

            migrationBuilder.DropColumn(
                name: "FinalStageParticipation_InternalUserId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreliminaryStageParticipation_InternalUserId",
                table: "Applications");
        }
    }
}
