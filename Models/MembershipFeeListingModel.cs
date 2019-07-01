using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Models
{
    public class MembershipFeeListingModel
    {
        public string Club { get; set; }
        public string User { get; set; }
        public string Amount { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public String PaymentTime { get; set; }
    }
}
