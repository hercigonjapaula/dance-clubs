using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Models
{
    public class ImageListingModel
    {
        public string GroupName { get; set; }
        public DateTime Published { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
    }
}
