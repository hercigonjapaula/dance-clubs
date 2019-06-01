using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Models
{
    public class CommentListingModel
    {
        public DateTime Published { get; set; }
        public string Author { get; set; }
        public string AuthorImage { get; set; }
        public string Content { get; set; }
    }
}
