using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RaspSecure.Migrations.LogsDb
{
    public partial class ReInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogEntitys",
                columns: table => new
                {
                    LogEntityId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(nullable: true),
                    AccessTime = table.Column<DateTimeOffset>(nullable: false),
                    IsSucceed = table.Column<bool>(nullable: false),
                    SecurityCodeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntitys", x => x.LogEntityId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEntitys");
        }
    }
}
