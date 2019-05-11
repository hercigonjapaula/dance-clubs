using System.ComponentModel.DataAnnotations;

namespace DanceClubs.Models
{
    public class NotificationInput
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite sadržaj.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Tekst", Prompt = "Proba u četvrtak!")]
        public string Content { get; set; }
    }
}
