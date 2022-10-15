using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Semicolon.OnlineJudge.Migrations
{
    public partial class InitTrackDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProblemId = table.Column<long>(nullable: false),
                    AuthorId = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    PointStatus = table.Column<string>(nullable: true),
                    CodeEncoded = table.Column<string>(nullable: true),
                    CompilerOutput = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tracks");
        }
    }
}
