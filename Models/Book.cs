using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Library_MVC_API.Models
{
    public partial class Book
    {
        public Book()
        {
            Bookings = new HashSet<Booking>();
        }

        [Display(Name = "Book ID")]
        [Required(ErrorMessage = "Book ID is required")]
        [RegularExpression(@"^[B][0-9]{5}?$", ErrorMessage = "Book ID should be B followed by five digits")]
        public string Bid { get; set; } = null!;
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        
        public string? Bname { get; set; }

        [Required(ErrorMessage = "Author name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string? Author { get; set; }
        [Display(Name ="Genre")]
        [Required(ErrorMessage = "Genre is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string? Jonour { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "Number of Copies is required")]
        [Display(Name = "Number of Copies")]
        public int? NoofCopies { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
