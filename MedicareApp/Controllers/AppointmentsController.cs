using MedicareApp.Data;
using MedicareApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedicareApp.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ------------------ Admin CRUD (already scaffolded) ------------------

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Payment);

            return View(await applicationDbContext.ToListAsync());
        }

        // ... (Keep Details, Create, Edit, Delete as you already have)

        // ------------------ Patient-Specific Actions ------------------

        // Book Appointment (GET)
        [Authorize(Roles = "Patient")]
        public IActionResult Book()
        {
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "Name");
            return View();
        }

        // Book Appointment (POST)
        [HttpPost]
        [Authorize(Roles = "Patient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int doctorId, DateTime appointmentDate)
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return RedirectToAction("CompleteProfile", "Patients");

            var appointment = new Appointment
            {
                DoctorID = doctorId,
                PatientID = patient.PatientID,
                AppointmentDate = appointmentDate,
                Status = "Pending"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyAppointments));
        }

        // View My Appointments
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                return RedirectToAction("CompleteProfile", "Patients");

            var myAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientID == patient.PatientID)
                .ToListAsync();

            return View(myAppointments);
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentID == id);
        }
        // POST: Appointments/Edit/5 (Patient Reschedule)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("AppointmentID,AppointmentDate,Notes")] Appointment appointment)
        {
            if (id != appointment.AppointmentID) return NotFound();

            if (!ModelState.IsValid)
            {
                var reload = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.AppointmentID == id);

                return View(reload);
            }

            var existing = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentID == id);

            if (existing == null) return NotFound();

            // ✅ Ensure only the patient who booked it can reschedule
            var userId = _userManager.GetUserId(User);
            if (existing.Patient.UserId != userId)
                return Forbid();

            // ✅ Update fields patient is allowed to change
            existing.AppointmentDate = appointment.AppointmentDate;
            existing.Notes = appointment.Notes;
            existing.Status = "Rescheduled";

            _context.Update(existing);
            await _context.SaveChangesAsync();

            // ✅ Redirect patient back to their appointments
            return RedirectToAction(nameof(MyAppointments));
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> MyPatients()
        {
            var userId = _userManager.GetUserId(User);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorID == doctor.DoctorID)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Confirm(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Confirmed";
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyAppointments));
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Cancelled";
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyAppointments));
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentID == id);

            if (appointment == null) return NotFound();

            // Only allow patient who booked it to reschedule
            var userId = _userManager.GetUserId(User);
            if (appointment.Patient.UserId != userId)
                return Forbid();

            return View(appointment);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Payment)
                .FirstOrDefaultAsync(m => m.AppointmentID == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentID == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
