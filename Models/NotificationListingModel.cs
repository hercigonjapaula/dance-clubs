using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Models
{
    public class NotificationListingModel
    {
        public int Id { get; set; }
        public string ClubName { get; set; }
        public string GroupName { get; set; }
        public DateTime Published { get; set; }
        public string Author { get; set; }
        public string AuthorImage { get; set; }
        public string Content { get; set; }
        public List<CommentListingModel> CommentListingModels { get; set; }

    }
}
