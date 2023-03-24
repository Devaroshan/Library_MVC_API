using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Library_MVC_API.Models
{
    public partial class Client
    {
        [Display(Name = "Client ID")]
        public int Cid { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string? Cname { get; set; }
        [Display(Name = "Number of Books")]
        public int? NoofBooks { get; set; }
        [Display(Name = "Customer Address")]
        [Required(ErrorMessage = "Address is required")]
        public string? Caddress { get; set; }
        [Display(Name = "Email ID")]
        [Required(ErrorMessage = "User id is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Enter valid User ID")]
        public string? Userid { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password doesn't match")]
        public string CPassword { get; set; }
        public decimal? TotalPrice { get; set; }

    }
}
