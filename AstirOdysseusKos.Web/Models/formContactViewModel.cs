﻿using System.ComponentModel.DataAnnotations;

namespace AstirOdysseusKos.Web.Models
{
    public class formContactViewModel
    {

        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Surname { get; set; }
        [Required]
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [Required]
        // [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", ErrorMessage = "Invalid Telephone Number")]
        [StringLength(14)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Number")]
        public string? Telephone { get; set; }
        public string? Message { get; set; }

        public bool ReceiveUpdates { get; set; } = false;
        [Required]
        public bool Agree { get; set; }
    }
}
