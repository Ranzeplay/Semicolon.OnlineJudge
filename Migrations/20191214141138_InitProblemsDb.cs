using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Semicolon.OnlineJudge.Migrations
{
    public partial class InitProblemsDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JudgeProfile",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestDatas = table.Column<string>(nullable: true),
                    MemoryLimit = table.Column<double>(nullable: false),
                    TimeLimit = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudgeProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PassRate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Submit = table.Column<long>(nullable: false),
                    Pass = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassRate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Input = table.Column<string>(nullable: true),
                    Output = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestData", x => x.Id);
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
                    ExampleDataId = table.Column<long>(nullable: true),
                    JudgeProfileId = table.Column<long>(nullable: true),
                    PassRateId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Problems_TestData_ExampleDataId",
                        column: x => x.ExampleDataId,
                        principalTable: "TestData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Problems_JudgeProfile_JudgeProfileId",
                        column: x => x.JudgeProfileId,
                        principalTable: "JudgeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Problems_PassRate_PassRateId",
                        column: x => x.PassRateId,
                        principalTable: "PassRate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ExampleDataId",
                table: "Problems",
                column: "ExampleDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_JudgeProfileId",
                table: "Problems",
                column: "JudgeProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_PassRateId",
                table: "Problems",
                column: "PassRateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Problems");

            migrationBuilder.DropTable(
                name: "TestData");

            migrationBuilder.DropTable(
                name: "JudgeProfile");

            migrationBuilder.DropTable(
                name: "PassRate");
        }
    }
}
