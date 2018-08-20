using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuideHaven.Models
{
    public class Step
    {
        public int StepId { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string Images { get; set; }

        public int GuideId { get; set; }
        public Guide Guide { get; set; }
    }
}
