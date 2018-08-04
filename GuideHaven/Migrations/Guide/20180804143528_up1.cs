using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations.Guide
{
    public partial class up1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuideName",
                table: "Guide",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuideName",
                table: "Guide");
        }
    }
}
