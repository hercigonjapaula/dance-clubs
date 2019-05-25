using DanceClubs.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DanceClubs.Models
{
    public class ClubUsers
    {
        public int Id { get; set; }

        public List<string> ApplicationUserId { get; set; }

        public int ClubId { get; set; }

        public DateTime MemberFrom { get; set; }
        public DateTime MemberTo { get; set; }

        public List<MembershipFee> MembershipFees { get; set; }
    }
}
