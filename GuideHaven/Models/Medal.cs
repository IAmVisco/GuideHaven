using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class Medal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<AspNetUserMedals> Users { get; set; }
    }
}
