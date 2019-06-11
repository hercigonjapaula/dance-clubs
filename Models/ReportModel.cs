using System.Collections.Generic;

namespace DanceClubs.Models
{
    public class ReportModel
    {
        public int ClubId { get; set; }
        public int Year { get; set; }        
        public List<ClubReportModel> ClubList { get; set; }
        public List<YearModel> YearList { get; set; }
        public Dictionary<string, List<ReportListingModel>> Report { get; set; }
    }
}
