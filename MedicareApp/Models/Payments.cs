using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicareApp.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Method { get; set; }   // Dropdown values: "Cash", "Credit Card", "Debit Card"

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        // 🔹 Relationships
        public int AppointmentID { get; set; }
        public Appointment Appointment { get; set; }
    }
}

