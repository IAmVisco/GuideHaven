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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guide>().ToTable("Guide");
            modelBuilder.Entity<Step>().ToTable("Step");
            modelBuilder.Entity<Comment>().ToTable("Comment");
            modelBuilder.Entity<Like>().ToTable("Like");
            modelBuilder.Entity<Rating>().ToTable("Rating");
        }

        public Guide GetGuide(GuideContext context, int? id)
        {
            var guides = context.Guide.Include(g => g.GuideSteps).Include(g => g.Ratings).ToList();
            var guide = guides.FirstOrDefault(m => m.GuideId == id);
            guide.Comments = context.Comments.Where(g => g.GuideId == id).Include(g => g.Likes).ToList();
            return guide;
        }

        public List<Guide> GetGuides(GuideContext context, string owner)
        {
            var guides = context.Guide.Include(g => g.GuideSteps).Include(g => g.Comments).Include(g => g.Ratings).ToList();
            guides.RemoveAll(x => x.Owner != owner);
            return guides;
        }
    }
}
