using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GuideHaven.Migrations.Guide
{
    public partial class NewModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guide",
                columns: table => new
                {
                    GuideId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Owner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guide", x => x.GuideId);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Owner = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    GuideId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comment_Guide_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guide",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    RatingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Owner = table.Column<string>(nullable: true),
                    OwnerRating = table.Column<int>(nullable: false),
                    GuideId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_Rating_Guide_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guide",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Step",
                columns: table => new
                {
                    StepId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Header = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    GuideId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Step", x => x.StepId);
                    table.ForeignKey(
                        name: "FK_Step_Guide_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guide",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Like",
                columns: table => new
                {
                    LikeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Owner = table.Column<string>(nullable: true),
                    CommentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Like", x => x.LikeId);
                    table.ForeignKey(
                        name: "FK_Like_Comment_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comment",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_GuideId",
                table: "Comment",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_Like_CommentId",
                table: "Like",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_GuideId",
                table: "Rating",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_Step_GuideId",
                table: "Step",
                column: "GuideId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Like");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "Step");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Guide");
        }
    }
}
