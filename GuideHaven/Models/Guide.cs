﻿using System;
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
        [StringLength(40, ErrorMessage = "LengthWarning", MinimumLength = 6)]
        [Display(Name = "Title")]
        public string GuideName { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [Required]
        public string Category { get; set; }
        public int Views { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual List<GuideTag> GuideTags { get; set; }
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
            if (Ratings.Count > 0)
                return temp / Ratings.Count;
            else
                return 0;
        }
    }
}
