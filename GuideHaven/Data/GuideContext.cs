using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GuideHaven.Models
{
    public class GuideContext : DbContext
    {
        public GuideContext(DbContextOptions<GuideContext> options)
            : base(options)
        {
        }

        public DbSet<Guide> Guide { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guide>().ToTable("Guide");
            modelBuilder.Entity<Step>().ToTable("Step");
            modelBuilder.Entity<Comment>().ToTable("Comment");
            modelBuilder.Entity<Like>().ToTable("Like");
            modelBuilder.Entity<Rating>().ToTable("Rating");

            modelBuilder.Entity<GuideTag>()
                .HasKey(t => new { t.GuideId, t.TagId });

            modelBuilder.Entity<GuideTag>()
                .HasOne(pt => pt.Guide)
                .WithMany(p => p.GuideTags)
                .HasForeignKey(pt => pt.GuideId);

            modelBuilder.Entity<GuideTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.GuideTags)
                .HasForeignKey(pt => pt.TagId);
        }

        public Guide GetGuide(GuideContext context, int? id)
        {
            var guides = context.Guide.Include(g => g.GuideSteps).Include(g => g.Ratings).Include(g => g.GuideTags).ToList();
            var guide = guides.FirstOrDefault(m => m.GuideId == id);
            guide.Comments = context.Comments.Where(g => g.GuideId == id).Include(g => g.Likes).ToList();
            return guide;
        }

        public List<Guide> GetGuides(GuideContext context, string owner = null)
        {
            var guides = context.Guide.Include(g => g.GuideSteps).Include(g => g.Comments).Include(g => g.Ratings).ToList();
            if (owner != null)
                guides.RemoveAll(x => x.Owner != owner);
            return guides;
        }

        public void SaveTags(GuideContext context, List<Tag> tags)
        {
            foreach (var item in tags)
            {
                if (context.Tags.Contains(item))
                {
                    context.Tags.FirstOrDefault(x => x.TagId == item.TagId).GuideTags.AddRange(item.GuideTags);
                }
                else
                    context.Tags.Add(item);
            }
        }
    }
}
