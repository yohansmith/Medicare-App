using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicareApp.Data;
using MedicareApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MedicareApp.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PaymentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Payments (Admin can see all payments)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Appointment.Patient)
                .ToListAsync();

            return View(payments);
        }

        // ✅ GET: Payments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            return View(payment);
        }

        // ✅ POST: Payments/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentID,Amount,Method,Date,AppointmentID")] Payment payment)
        {
            if (id != payment.PaymentID) return NotFound();

            if (!ModelState.IsValid) return View(payment);

            try
            {
                _context.Update(payment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(payment.PaymentID)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ GET: Payments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(m => m.PaymentID == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        // ✅ POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // 🔹 Simulated Pay Now (one-click payment)
        [HttpPost]
        [Authorize(Roles = "Patient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayNow(int id)
        {
            var userId = _userManager.GetUserId(User);
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentID == id);

            if (appointment == null || appointment.Patient.UserId != userId)
                return Forbid();

            // Prevent duplicate payment
            if (appointment.PaymentID != null)
                return RedirectToAction("MyAppointments", "Appointments");

            // Step 1: create and save payment first
            var payment = new Payment
            {
                Amount = appointment.Doctor?.Fees ?? 0,
                Method = "Cash",
                Date = DateTime.Now
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(); // ensures PaymentID is generated

            // Step 2: link appointment to payment
            appointment.PaymentID = payment.PaymentID;
            appointment.Status = "Paid";

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            // Step 3: reload MyAppointments to reflect changes
            return RedirectToAction("MyAppointments", "Appointments");
        }


        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(m => m.PaymentID == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentID == id);
        }
    }
}
