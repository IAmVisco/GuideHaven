using GuideHaven.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class AspNetUserMedals
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int MedalId { get; set; }
        public Medal Medal { get; set; }
    }
}
