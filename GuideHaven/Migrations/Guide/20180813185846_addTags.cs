using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations.Guide
{
    public partial class addTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GuideName",
                table: "Guide",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Guide",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Guide");

            migrationBuilder.AlterColumn<string>(
                name: "GuideName",
                table: "Guide",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 60);
        }
    }
}
