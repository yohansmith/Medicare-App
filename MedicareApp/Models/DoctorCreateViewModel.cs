using System;
using System.ComponentModel.DataAnnotations;

namespace MedicareApp.Models
{
    public class DoctorCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Specialty { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Fees { get; set; }

        // Change to DateTime
        [Required]
        public string Availability { get; set; }


        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
