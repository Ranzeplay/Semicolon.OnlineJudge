using Microsoft.EntityFrameworkCore.Migrations;

namespace Semicolon.OnlineJudge.Migrations
{
    public partial class AddMoreCompilerOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Tracks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Tracks");
        }
    }
}
