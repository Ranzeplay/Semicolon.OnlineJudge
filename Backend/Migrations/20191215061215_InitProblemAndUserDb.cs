using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Semicolon.OnlineJudge.Migrations
{
    public partial class InitProblemAndUserDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OJUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    ProblemsPassedId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OJUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Problems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    AuthorId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PublishTime = table.Column<DateTime>(nullable: false),
                    ExampleData = table.Column<string>(nullable: true),
                    JudgeProfile = table.Column<string>(nullable: true),
                    PassRate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OJUsers");

            migrationBuilder.DropTable(
                name: "Problems");
        }
    }
}
