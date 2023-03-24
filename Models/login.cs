using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Library_MVC_API.Models
{
    public class login
    {
        [Display(Name = "Email ID")]
        [Required(ErrorMessage = "User id is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Enter valid User ID")]
        public string? Userid { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }
    }
}
