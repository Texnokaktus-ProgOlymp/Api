using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Addconteststate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreliminaryStage_State",
                table: "Contests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinalStage_State",
                table: "Contests",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalStage_State",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "PreliminaryStage_State",
                table: "Contests");
        }
    }
}
