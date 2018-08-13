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
        [Required]
        [StringLength(60, ErrorMessage = "LengthWarning", MinimumLength = 6)]
        [Display(Name = "Title")]
        public string GuideName { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Tags { get; set; }

        public virtual List<Step> GuideSteps { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Rating> Ratings { get; set; }

        public double GetRating()
        {
            float temp = 0;
            foreach (var item in Ratings)
            {
                temp += item.OwnerRating;
            }
            return temp / Ratings.Count;
        }
    }
}
