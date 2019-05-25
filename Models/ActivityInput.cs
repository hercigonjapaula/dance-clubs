using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DanceClubs.Models
{
    public class ActivityInput
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite ime kluba.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Ime kluba", Prompt = "JazzElle")]
        public string ClubName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite ime grupe.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Ime grupe", Prompt = "Contemporary")]
        public string GroupName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite vrstu aktivnosti.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Vrsta aktivnosti", Prompt = "Proba")]
        public string Type { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite datum početka aktivnosti.")]
        [Display(Name = "Datum početka", Prompt = "12.5.2019.")]
        public string StartDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite vrijeme početka aktivnosti.")]
        [Display(Name = "Vrijeme početka", Prompt = "20:30")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "Unesite datum kraja aktivnosti.")]
        [Display(Name = "Datum završetka", Prompt = "12.5.2019.")]
        public string EndDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite vrijeme završetka aktivnosti.")]
        [Display(Name = "Vrijeme završetka", Prompt = "22:00")]
        public string EndTime { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite mjesto aktivnosti.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Mjesto", Prompt = "Husar&Tomčić škola pjevanja")]
        public string Location { get; set; }
    }
}
