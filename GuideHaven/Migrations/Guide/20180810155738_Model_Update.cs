using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations.Guide
{
    public partial class Model_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Guide_GuideId",
                table: "Comment");

            migrationBuilder.AlterColumn<int>(
                name: "GuideId",
                table: "Comment",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Guide_GuideId",
                table: "Comment",
                column: "GuideId",
                principalTable: "Guide",
                principalColumn: "GuideId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Guide_GuideId",
                table: "Comment");

            migrationBuilder.AlterColumn<int>(
                name: "GuideId",
                table: "Comment",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Guide_GuideId",
                table: "Comment",
                column: "GuideId",
                principalTable: "Guide",
                principalColumn: "GuideId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
