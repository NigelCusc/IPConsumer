using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class ProgressReportModel
    {
        public Guid Guid { get; set; }
        public int PercentageComplete { get; set; } = 0;
        public List<IPDetails> IPDetailsCompleted { get; set; } = new List<IPDetails>();
    }
}
