using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DanceClubs.Models
{
    public class ClubInput
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite naziv kluba.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Naziv", Prompt = "Toi Toi Toi")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Unesite adresu kluba.")]
        [Display(Name = "Adresa", Prompt = "Zagorska ulica 15, Zagreb")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Unesite emailove vlasnika.")]
        [Display(Name = "Emailovi vlasnika", Prompt = "and5678@gmail.com")]
        public List<string> OwnersEmails{ get; set; }


    }
}
