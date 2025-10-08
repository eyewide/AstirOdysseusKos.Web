using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace AstirOdysseusKos.Web.Models
{
    public class formReservationViewModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Surname { get; set; }
        [Required]
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [Required]
        public string? Telephone { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        [Required]
        public string? ArrivalDate { get; set; }
        [Required]
        public string? DepartureDate { get; set; }
        public string? Nights { get; set; }
        //public List<SelectListItem>? accomTypes { get; set; }

        //[Required]
        public string? AccommodationType { get; set; }
        public List<SelectListItem>? dropNumbers { get; set; }
        public List<SelectListItem>? dropNumbers2 { get; set; }

        [Required]
        public string? NoAdults { get; set; }
        public string? NoChildren { get; set; }
        //public List<SelectListItem>? whereDidYouFindUs { get; set; }
        // public string? FindUsFrom { get; set; }
        public string? Comments { get; set; }
        public bool? ReceiveUpdates { get; set; } = false;

        [Required]
        public bool Agree { get; set; }
    }
}
