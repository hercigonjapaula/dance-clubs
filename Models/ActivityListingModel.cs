﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Models
{
    public class ActivityListingModel
    {
        public string Description { get; set; }
        public string ActivityType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Location { get; set; }
        public string Author { get; set; }
        public string Group { get; set; }
    }
}
