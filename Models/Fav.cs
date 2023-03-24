using System.ComponentModel.DataAnnotations;

namespace Library_MVC_API.Models
{
    public class Fav
    {
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string? Author { get; set; }
        [Display(Name = "Genre")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Use letters only please")]
        public string? Jonour { get; set; }
    }
}
