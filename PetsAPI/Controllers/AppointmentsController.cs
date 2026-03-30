using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models;

namespace PetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly PetsDbContext _context;

        public AppointmentsController(PetsDbContext context)
        {
            _context = context;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            // 標記即將到來的預約（24小時內）
            var now = DateTime.UtcNow;
            foreach (var appointment in appointments)
            {
                if (appointment.AppointmentDate > now && 
                    (appointment.AppointmentDate - now).TotalHours < 24)
                {
                    appointment.Status = "即將到來";
                }
            }

            return appointments;
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            if (string.IsNullOrEmpty(appointment.Status))
            {
                appointment.Status = "預約中";
            }

            // 確保日期以 UTC 格式儲存到 PostgreSQL
            appointment.AppointmentDate = DateTime.SpecifyKind(appointment.AppointmentDate, DateTimeKind.Utc);

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.ID }, appointment);
        }

        // PUT: api/Appointments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, Appointment appointment)
        {
            if (id != appointment.ID)
            {
                return BadRequest();
            }

            // 確保日期以 UTC 格式儲存到 PostgreSQL
            appointment.AppointmentDate = DateTime.SpecifyKind(appointment.AppointmentDate, DateTimeKind.Utc);

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.ID == id);
        }
    }
}
