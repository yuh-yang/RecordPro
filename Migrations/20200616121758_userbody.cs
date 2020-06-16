using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecordPRO.Migrations
{
    public partial class userbody : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBody",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    weight = table.Column<float>(nullable: false),
                    shoulder = table.Column<float>(nullable: false),
                    height = table.Column<float>(nullable: false),
                    bust = table.Column<float>(nullable: false),
                    waist = table.Column<float>(nullable: false),
                    hips = table.Column<float>(nullable: false),
                    dateTime = table.Column<DateTime>(nullable: false),
                    userid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBody", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBody");
        }
    }
}
