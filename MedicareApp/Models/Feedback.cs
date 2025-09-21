using System;
using System.ComponentModel.DataAnnotations;

namespace MedicareApp.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [Required]
        public string Message { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }  // 1–5 stars

        public DateTime Date { get; set; } = DateTime.Now;

        // Link to Patient
        public int? PatientID { get; set; }
        public Patient Patient { get; set; }
    }
}
