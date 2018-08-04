using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        public string Owner { get; set; }

        public int CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}
