using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations
{
    public partial class UserUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medal",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserMedals",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    MedalId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserMedals", x => new { x.UserId, x.MedalId });
                    table.ForeignKey(
                        name: "FK_AspNetUserMedals_Medal_MedalId",
                        column: x => x.MedalId,
                        principalTable: "Medal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserMedals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserMedals_MedalId",
                table: "AspNetUserMedals",
                column: "MedalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetUserMedals");

            migrationBuilder.DropTable(
                name: "Medal");
        }
    }
}
