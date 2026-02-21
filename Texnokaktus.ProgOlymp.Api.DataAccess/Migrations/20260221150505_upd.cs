using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class upd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_ContestStages_FinalStageId",
                table: "Contests");

            migrationBuilder.DropForeignKey(
                name: "FK_Contests_ContestStages_PreliminaryStageId",
                table: "Contests");

            migrationBuilder.DropIndex(
                name: "IX_Contests_FinalStageId",
                table: "Contests");

            migrationBuilder.DropIndex(
                name: "IX_Contests_PreliminaryStageId",
                table: "Contests");

            migrationBuilder.RenameColumn(
                name: "PreliminaryStageId",
                table: "Contests",
                newName: "PreliminaryStage_YandexContestId");

            migrationBuilder.AddColumn<string>(
                name: "PreliminaryStage_Name",
                table: "Contests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PreliminaryStage_ContestStart",
                table: "Contests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PreliminaryStage_ContestFinish",
                table: "Contests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PreliminaryStage_Duration",
                table: "Contests",
                type: "bigint",
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "FinalStageId",
                table: "Contests",
                newName: "FinalStage_YandexContestId");

            migrationBuilder.AddColumn<string>(
                name: "FinalStage_Name",
                table: "Contests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FinalStage_ContestStart",
                table: "Contests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FinalStage_ContestFinish",
                table: "Contests",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FinalStage_Duration",
                table: "Contests",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Contests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                                 UPDATE [c]
                                 SET [c].[Title]                          = [c].[Name],
                                     [c].[Name]                           = CONCAT('olymp', RIGHT([c].[Name], 2)),
                                     [c].[PreliminaryStage_Name]          = [ps].[Name],
                                     [c].[PreliminaryStage_ContestStart]  = [ps].[ContestStart],
                                     [c].[PreliminaryStage_ContestFinish] = [ps].[ContestStart],
                                     [c].[PreliminaryStage_Duration]      = [ps].[Duration],
                                     [c].[FinalStage_Name]                = [fs].[Name],
                                     [c].[FinalStage_ContestStart]        = [fs].[ContestStart],
                                     [c].[FinalStage_ContestFinish]       = [fs].[ContestStart],
                                     [c].[FinalStage_Duration]            = [fs].[Duration]
                                 FROM [Contests] [c]
                                          LEFT JOIN [ContestStages] [ps] ON [ps].[Id] = [c].[PreliminaryStage_YandexContestId]
                                          LEFT JOIN [ContestStages] [fs] ON [fs].[Id] = [c].[FinalStage_YandexContestId];
                                 """);

            migrationBuilder.DropTable(
                name: "ContestStages");

            migrationBuilder.AddColumn<long>(
                name: "FinalStageParticipation_ContestUserId",
                table: "Applications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FinalStageParticipation_Finish",
                table: "Applications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FinalStageParticipation_Start",
                table: "Applications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinalStageParticipation_State",
                table: "Applications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PreliminaryStageParticipation_ContestUserId",
                table: "Applications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PreliminaryStageParticipation_Finish",
                table: "Applications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PreliminaryStageParticipation_Start",
                table: "Applications",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreliminaryStageParticipation_State",
                table: "Applications",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalStage_ContestFinish",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "FinalStage_ContestStart",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "FinalStage_Duration",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "FinalStage_Name",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "FinalStage_YandexContestId",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "PreliminaryStage_ContestFinish",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "PreliminaryStage_ContestStart",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "PreliminaryStage_Name",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "FinalStageParticipation_ContestUserId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "FinalStageParticipation_Finish",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "FinalStageParticipation_Start",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "FinalStageParticipation_State",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreliminaryStageParticipation_ContestUserId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreliminaryStageParticipation_Finish",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreliminaryStageParticipation_Start",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PreliminaryStageParticipation_State",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "PreliminaryStage_YandexContestId",
                table: "Contests",
                newName: "PreliminaryStageId");

            migrationBuilder.RenameColumn(
                name: "PreliminaryStage_Duration",
                table: "Contests",
                newName: "FinalStageId");

            migrationBuilder.CreateTable(
                name: "ContestStages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ContestFinish = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ContestStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Duration = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestStages", x => x.Id);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_ContestStages_FinalStageId",
                table: "Contests",
                column: "FinalStageId",
                principalTable: "ContestStages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_ContestStages_PreliminaryStageId",
                table: "Contests",
                column: "PreliminaryStageId",
                principalTable: "ContestStages",
                principalColumn: "Id");
        }
    }
}
