using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fall2024_Assignment3_separal.Migrations
{
    /// <inheritdoc />
    public partial class ExistingBookClicksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
           name: "IX_BookClicks_BookID",
           table: "BookClicks",
           column: "BookID");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookClicks_BookID",
                table: "BookClicks");
        }

    }
}