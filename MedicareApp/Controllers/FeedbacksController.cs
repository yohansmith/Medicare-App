using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicareApp.Data;
using MedicareApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MedicareApp.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FeedbacksController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Admin: View all feedback
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Patient)
                .ToListAsync();
            return View(feedbacks);
        }

        // ✅ Patient: View own feedback
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyFeedbacks()
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("CompleteProfile", "Patients");

            var feedbacks = await _context.Feedbacks
                .Where(f => f.PatientID == patient.PatientID)
                .ToListAsync();

            return View(feedbacks);
        }

        // ✅ Patient: Give feedback
        [Authorize(Roles = "Patient")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Message,Rating")] Feedback feedback)
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("CompleteProfile", "Patients");

            if (ModelState.IsValid)
            {
                feedback.PatientID = patient.PatientID;
                feedback.Date = DateTime.Now;

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyFeedbacks));
            }
            return View(feedback);
        }

        // ✅ Admin: View feedback details
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var feedback = await _context.Feedbacks
                .Include(f => f.Patient)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);

            if (feedback == null) return NotFound();

            return View(feedback);
        }

        // ✅ Admin: Delete feedback
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var feedback = await _context.Feedbacks
                .Include(f => f.Patient)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);

            if (feedback == null) return NotFound();

            return View(feedback);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
