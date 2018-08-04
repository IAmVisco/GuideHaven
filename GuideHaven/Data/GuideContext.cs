using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GuideHaven.Models
{
    public class GuideContext : DbContext
    {
        public GuideContext (DbContextOptions<GuideContext> options)
            : base(options)
        {
        }

        public DbSet<GuideHaven.Models.Guide> Guide { get; set; }
    }
}
