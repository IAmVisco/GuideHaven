using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class Tag
    {
        public string TagId { get; set; }

        public List<GuideTag> GuideTags { get; set; }
    }
}
