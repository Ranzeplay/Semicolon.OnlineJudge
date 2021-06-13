using Microsoft.EntityFrameworkCore.Migrations;

namespace Semicolon.OnlineJudge.Migrations
{
    public partial class AddCustomLanguageSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Tracks");

            migrationBuilder.AddColumn<string>(
                name: "LanguageId",
                table: "Tracks",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Tracks");

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Tracks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
