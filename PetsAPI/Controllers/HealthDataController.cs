using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models;
using PetsAPI.Services;

namespace PetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthDataController : ControllerBase
    {
        private readonly PetsDbContext _context;
        private readonly IAlertService _alertService;

        public HealthDataController(PetsDbContext context, IAlertService alertService)
        {
            _context = context;
            _alertService = alertService;
        }

        // GET: api/HealthData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HealthData>>> GetHealthData()
        {
            return await _context.HealthData
                .OrderByDescending(c => c.Timestamp)
                .Take(100)
                .ToListAsync();
        }

        // GET: api/HealthData/pet/5
        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IEnumerable<HealthData>>> GetHealthDataByPet(int petId)
        {
            return await _context.HealthData
                .Where(c => c.PetID == petId)
                .OrderByDescending(c => c.Timestamp)
                .ToListAsync();
        }

        // POST: api/HealthData
        [HttpPost]
        public async Task<ActionResult<HealthData>> CreateHealthData(HealthData healthData)
        {
            healthData.Timestamp = DateTime.Now;
            _context.HealthData.Add(healthData);
            await _context.SaveChangesAsync();

            // 檢查異常並產生警報
            var alerts = await _alertService.CheckHealthDataAlertsAsync(healthData);

            return CreatedAtAction(nameof(GetHealthData), new { id = healthData.ID }, new { healthData, alerts });
        }
    }
}
