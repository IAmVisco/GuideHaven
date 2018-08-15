using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations.Guide
{
    public partial class AddViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Guide");

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "Guide",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "Guide");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Guide",
                nullable: true);
        }
    }
}
