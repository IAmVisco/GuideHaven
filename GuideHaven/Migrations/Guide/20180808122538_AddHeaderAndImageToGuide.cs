using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations.Guide
{
    public partial class AddHeaderAndImageToGuide : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Guide",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Guide",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Guide");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Guide");
        }
    }
}
