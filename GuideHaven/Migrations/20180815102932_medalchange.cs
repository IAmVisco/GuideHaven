using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations
{
    public partial class medalchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserMedals_Medal_MedalId",
                table: "AspNetUserMedals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medal",
                table: "Medal");

            migrationBuilder.RenameTable(
                name: "Medal",
                newName: "Medals");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "MedalId",
                table: "AspNetUserMedals",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Medals",
                nullable: false,
                oldClrType: typeof(string))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medals",
                table: "Medals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserMedals_Medals_MedalId",
                table: "AspNetUserMedals",
                column: "MedalId",
                principalTable: "Medals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserMedals_Medals_MedalId",
                table: "AspNetUserMedals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medals",
                table: "Medals");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "Medals",
                newName: "Medal");

            migrationBuilder.AlterColumn<string>(
                name: "MedalId",
                table: "AspNetUserMedals",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Medal",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medal",
                table: "Medal",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserMedals_Medal_MedalId",
                table: "AspNetUserMedals",
                column: "MedalId",
                principalTable: "Medal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
