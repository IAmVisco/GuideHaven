using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations.Guide
{
    public partial class AddImagesToSteps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "Step",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Guide",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "Step");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Guide",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
