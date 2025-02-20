using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContestStages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContestStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ContestFinish = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "varchar(900)", unicode: false, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultAvatar = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_Login", x => x.Login);
                });

            migrationBuilder.CreateTable(
                name: "Contests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PreliminaryStageId = table.Column<long>(type: "bigint", nullable: true),
                    FinalStageId = table.Column<long>(type: "bigint", nullable: true),
                    RegistrationStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RegistrationFinish = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contests_ContestStages_FinalStageId",
                        column: x => x.FinalStageId,
                        principalTable: "ContestStages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contests_ContestStages_PreliminaryStageId",
                        column: x => x.PreliminaryStageId,
                        principalTable: "ContestStages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ContestId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patronym = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Snils = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchoolName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Parent_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parent_LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parent_Patronym = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parent_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Parent_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Teacher_School = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Teacher_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Teacher_LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Teacher_Patronym = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Teacher_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Teacher_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalDataConsent = table.Column<bool>(type: "bit", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.UniqueConstraint("AK_Applications_ContestId_UserId", x => new { x.ContestId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Applications_Contests_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_RegionId",
                table: "Applications",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_UserId",
                table: "Applications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contests_FinalStageId",
                table: "Contests",
                column: "FinalStageId",
                unique: true,
                filter: "[FinalStageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Contests_PreliminaryStageId",
                table: "Contests",
                column: "PreliminaryStageId",
                unique: true,
                filter: "[PreliminaryStageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                table: "Regions",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Contests");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ContestStages");
        }
    }
}
