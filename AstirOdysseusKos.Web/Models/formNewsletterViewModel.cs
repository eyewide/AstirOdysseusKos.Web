using System.ComponentModel.DataAnnotations;

namespace AstirOdysseusKos.Web.Models
{
    public class formNewsletterViewModel
    {

        [Required]
        public string? NameSec { get; set; }
        [Required]
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        public string? EmailSec { get; set; }
        [Required]
        public bool AgreeSec { get; set; }
    }


}
