using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Library_MVC_API.Models
{
    public class Cpwd
    {
        [Key]
        [Display(Name = "Old Password")]
        [Required(ErrorMessage = "Enter Old Password")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = " ")]
        public string CPassword { get; set; }
    }
}
