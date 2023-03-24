using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Library_MVC_API.Models
{
    public partial class Booking
    {
        [Display(Name = "Booking ID")]
        public int Bkid { get; set; }
        [Display(Name = "Book Name")]
        /*[Required(ErrorMessage = "Name is required")]*/
        public string? Bname { get; set; }
        public string? Author { get; set; }
        public string? Jonour { get; set; }
        public decimal? Price { get; set; }
        [Display(Name = "Book ID")]
        public string? Bid { get; set; }
        [Display(Name = "Copies Purchased")]
        public int? NoofCopies { get; set; }
        [Display(Name = "Total Price")]
        public decimal? TotalPrice { get; set; }
        [Display(Name = "Client ID")]
        [Required(ErrorMessage = "Client Id is required")]
        public int? Cid { get; set; }

        [Display(Name = "Payment Status")]
        public int? Status { get; set; }


        public virtual Book? BidNavigation { get; set; }
    }
}
