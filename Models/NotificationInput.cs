using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DanceClubs.Models
{
    public class NotificationInput
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite sadržaj.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Tekst", Prompt = "Proba u četvrtak!")]
        public string Content { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite imena grupa.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Grupe", Prompt = "Contemporary Jazz")]
        public List<string> GroupNames { get; set; }
    }
}
