using System;
using System.ComponentModel.DataAnnotations;

namespace MedicareApp.Models
{
    public class Patient
    {
        [Key]
        public int PatientID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required]
        public string Contact { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Link to Identity User
        public string? UserId { get; set; }
    }
}
