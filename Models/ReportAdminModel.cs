using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Models
{
    public class ReportAdminModel
    {
        public int Year { get; set; }  
        public List<YearModel> YearList { get; set; }
        public Dictionary<string, List<ReportListingModel>> Report { get; set; }
    }
}
