using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class GuideTag
    {
        public int GuideId { get; set; }
        public Guide Guide { get; set; }

        public string TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
