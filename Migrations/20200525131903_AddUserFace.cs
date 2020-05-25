using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecordPRO.Migrations
{
    public partial class AddUserFace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFace",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    filepath = table.Column<string>(nullable: true),
                    gender = table.Column<int>(nullable: false),
                    smile = table.Column<int>(nullable: false),
                    age = table.Column<int>(nullable: false),
                    emotion = table.Column<string>(nullable: true),
                    beauty = table.Column<string>(nullable: true),
                    facetoken = table.Column<string>(nullable: true),
                    userid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFace", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFace");
        }
    }
}
