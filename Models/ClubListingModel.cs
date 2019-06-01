using DanceClubs.Data.Models;
using System.Collections.Generic;

namespace DanceClubs.Models
{
    public class ClubListingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserListingModel> Users { get; set; }
    }
}
