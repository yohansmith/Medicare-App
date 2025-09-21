using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicareApp.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";  // Pending, Confirmed, Cancelled

        public string? Notes { get; set; }  // nullable

        // 🔹 Relationships

        [Required]
        public int DoctorID { get; set; }
        [ForeignKey("DoctorID")]
        public Doctor Doctor { get; set; }

        [Required]
        public int PatientID { get; set; }
        [ForeignKey("PatientID")]
        public Patient Patient { get; set; }

        // Optional link to Payment
        public int? PaymentID { get; set; }
        [ForeignKey("PaymentID")]
        public Payment Payment { get; set; }
    }
}
