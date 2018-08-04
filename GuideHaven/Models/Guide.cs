using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class Guide
    {
        public int GuideId { get; set; }
        public string GuideName { get; set; }
        public string Owner { get; set; }

        public List<Step> GuideSteps { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}
