using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedicareApp.Data;

namespace MedicareApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 📊 Appointments grouped by month
            var appointmentsByMonth = _context.Appointments
                .GroupBy(a => new { a.AppointmentDate.Year, a.AppointmentDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            // 💰 Total payments
            var totalPayments = _context.Payments.Sum(p => p.Amount);

            // 👥 Patient statistics
            var totalPatients = _context.Patients.Count();

            // 👨‍⚕️ Doctor statistics
            var totalDoctors = _context.Doctors.Count();

            // ⭐ Feedback statistics
            var totalFeedbacks = _context.Feedbacks.Count();
            var avgFeedbackRating = _context.Feedbacks.Any()
                ? _context.Feedbacks.Average(f => f.Rating)
                : 0;

            // Pass to view
            ViewBag.AppointmentsByMonth = appointmentsByMonth;
            ViewBag.TotalPayments = totalPayments;
            ViewBag.TotalPatients = totalPatients;
            ViewBag.TotalDoctors = totalDoctors;
            ViewBag.TotalFeedbacks = totalFeedbacks;
            ViewBag.AvgFeedbackRating = avgFeedbackRating;

            return View();
        }
    }
}
