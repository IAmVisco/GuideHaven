using GuideHaven.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public List<AspNetUserMedals> Medals { get; set; }
    }
}
