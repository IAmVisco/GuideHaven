using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class Guide
    {
        public int GuideId { get; set; }
        [Display(Name = "Guide Name")]
        public string GuideName { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public virtual List<Step> GuideSteps { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Rating> Ratings { get; set; }
    }
}
