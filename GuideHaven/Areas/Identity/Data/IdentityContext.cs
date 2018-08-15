using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuideHaven.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GuideHaven.Models
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        public DbSet<Medal> Medals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            //builder.Entity<Medal>().ToTable("Medal");

            builder.Entity<AspNetUserMedals>()
                .HasKey(t => new { t.UserId, t.MedalId });

            builder.Entity<AspNetUserMedals>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.Medals)
                .HasForeignKey(pt => pt.UserId);

            builder.Entity<AspNetUserMedals>()
                .HasOne(pt => pt.Medal)
                .WithMany(t => t.Users)
                .HasForeignKey(pt => pt.MedalId);
        }
    }
}
