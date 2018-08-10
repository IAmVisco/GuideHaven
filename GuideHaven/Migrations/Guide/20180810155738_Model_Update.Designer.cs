﻿// <auto-generated />
using System;
using GuideHaven.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GuideHaven.Migrations.Guide
{
    [DbContext(typeof(GuideContext))]
    [Migration("20180810155738_Model_Update")]
    partial class Model_Update
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GuideHaven.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreationTime");

                    b.Property<int>("GuideId");

                    b.Property<string>("Owner");

                    b.HasKey("CommentId");

                    b.HasIndex("GuideId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("GuideHaven.Models.Guide", b =>
                {
                    b.Property<int>("GuideId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("GuideName");

                    b.Property<string>("Image");

                    b.Property<string>("Owner");

                    b.HasKey("GuideId");

                    b.ToTable("Guide");
                });

            modelBuilder.Entity("GuideHaven.Models.Like", b =>
                {
                    b.Property<int>("LikeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CommentId");

                    b.Property<string>("Owner");

                    b.HasKey("LikeId");

                    b.HasIndex("CommentId");

                    b.ToTable("Like");
                });

            modelBuilder.Entity("GuideHaven.Models.Rating", b =>
                {
                    b.Property<int>("RatingId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GuideId");

                    b.Property<string>("Owner");

                    b.Property<int>("OwnerRating");

                    b.HasKey("RatingId");

                    b.HasIndex("GuideId");

                    b.ToTable("Rating");
                });

            modelBuilder.Entity("GuideHaven.Models.Step", b =>
                {
                    b.Property<int>("StepId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<int>("GuideId");

                    b.Property<string>("Header");

                    b.HasKey("StepId");

                    b.HasIndex("GuideId");

                    b.ToTable("Step");
                });

            modelBuilder.Entity("GuideHaven.Models.Comment", b =>
                {
                    b.HasOne("GuideHaven.Models.Guide", "Guide")
                        .WithMany("Comments")
                        .HasForeignKey("GuideId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GuideHaven.Models.Like", b =>
                {
                    b.HasOne("GuideHaven.Models.Comment", "Comment")
                        .WithMany("Likes")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GuideHaven.Models.Rating", b =>
                {
                    b.HasOne("GuideHaven.Models.Guide", "Guide")
                        .WithMany("Ratings")
                        .HasForeignKey("GuideId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GuideHaven.Models.Step", b =>
                {
                    b.HasOne("GuideHaven.Models.Guide", "Guide")
                        .WithMany("GuideSteps")
                        .HasForeignKey("GuideId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}