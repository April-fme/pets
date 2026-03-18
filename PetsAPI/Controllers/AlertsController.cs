using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models;

namespace PetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly PetsDbContext _context;

        public AlertsController(PetsDbContext context)
        {
            _context = context;
        }

        // GET: api/Alerts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlerts()
        {
            return await _context.Alerts
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Alerts/unresolved
        [HttpGet("unresolved")]
        public async Task<ActionResult<IEnumerable<Alert>>> GetUnresolvedAlerts()
        {
            return await _context.Alerts
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Alerts/pet/5
        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsByPet(int petId)
        {
            return await _context.Alerts
                .Where(a => a.PetID == petId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        // PUT: api/Alerts/5/resolve
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveAlert(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            alert.IsResolved = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Alerts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlert(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
