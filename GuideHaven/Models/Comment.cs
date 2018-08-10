using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Owner { get; set; }
        public DateTime CreationTime { get; set; }
        public string Content { get; set; }

        public int GuideId { get; set; }
        public Guide Guide { get; set; }
        public List<Like> Likes { get; set; }
    }
}
