using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations
{
    public partial class yahz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Medals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserMedals",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    MedalId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserMedals", x => new { x.UserId, x.MedalId });
                    table.ForeignKey(
                        name: "FK_AspNetUserMedals_Medals_MedalId",
                        column: x => x.MedalId,
                        principalTable: "Medals",
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
                name: "Medals");

        }
    }
}
