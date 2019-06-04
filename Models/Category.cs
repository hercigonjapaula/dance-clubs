using System.ComponentModel.DataAnnotations;

namespace DanceClubs.Models
{
    public enum Category
    {
        [Display(Name = "Treninzi")]
        Treninzi = 1,
        [Display(Name = "Probe")]
        Probe = 2,
        [Display(Name = "Nastupi")]
        Nastupi = 3,
        [Display(Name = "Članarine")]
        Članarine = 4
    }
}
