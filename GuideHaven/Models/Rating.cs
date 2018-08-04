using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public string Owner { get; set; }
        public int OwnerRating { get; set; }

        public int GuideId { get; set; }
        public Guide Guide { get; set; }
    }
}
