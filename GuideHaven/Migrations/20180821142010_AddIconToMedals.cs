using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations
{
    public partial class AddIconToMedals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Medals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Medals");
        }
    }
}
