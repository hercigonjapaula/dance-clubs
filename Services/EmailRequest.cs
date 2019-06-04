using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Services
{
    public class EmailRequest
    {
        [Required]
        public string ToAddress { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string  Body { get; set; }
        public MemoryStream Attachment { get; set; }  
        public string FileName { get; set; }
    }
}
