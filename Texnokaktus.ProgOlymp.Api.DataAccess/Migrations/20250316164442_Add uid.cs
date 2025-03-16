using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Texnokaktus.ProgOlymp.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Adduid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Uid",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uid",
                table: "Applications");
        }
    }
}
