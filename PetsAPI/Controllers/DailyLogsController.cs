using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models;
using PetsAPI.Services;

namespace PetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyLogsController : ControllerBase
    {
        private readonly PetsDbContext _context;
        private readonly IAlertService _alertService;

        public DailyLogsController(PetsDbContext context, IAlertService alertService)
        {
            _context = context;
            _alertService = alertService;
        }

        // GET: api/DailyLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DailyLog>>> GetDailyLogs()
        {
            return await _context.DailyLogs
                .OrderByDescending(d => d.CreatedAt)
                .Take(100)
                .ToListAsync();
        }

        // GET: api/DailyLogs/pet/5
        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IEnumerable<DailyLog>>> GetDailyLogsByPet(int petId)
        {
            return await _context.DailyLogs
                .Where(d => d.PetID == petId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        // POST: api/DailyLogs
        [HttpPost]
        public async Task<ActionResult<DailyLog>> CreateDailyLog(DailyLog dailyLog)
        {
            dailyLog.CreatedAt = DateTime.UtcNow;

            // 計算過去 7 筆平均食量
            var recentLogs = await _context.DailyLogs
                .Where(d => d.PetID == dailyLog.PetID)
                .OrderByDescending(d => d.CreatedAt)
                .Take(7)
                .ToListAsync();

            var avgFood = recentLogs.Any() ? recentLogs.Average(d => d.FoodAmount) : 0;

            _context.DailyLogs.Add(dailyLog);
            await _context.SaveChangesAsync();

            // 檢查異常並產生警報
            var alerts = await _alertService.CheckDailyLogAlertsAsync(dailyLog, avgFood);

            return CreatedAtAction(nameof(GetDailyLogs), new { id = dailyLog.ID }, new { dailyLog, alerts });
        }
    }
}
