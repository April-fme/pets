using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models;
using PetsAPI.Services;

namespace PetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollarDataController : ControllerBase
    {
        private readonly PetsDbContext _context;
        private readonly IAlertService _alertService;

        public CollarDataController(PetsDbContext context, IAlertService alertService)
        {
            _context = context;
            _alertService = alertService;
        }

        // GET: api/CollarData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CollarData>>> GetCollarData()
        {
            return await _context.CollarData
                .OrderByDescending(c => c.Timestamp)
                .Take(100)
                .ToListAsync();
        }

        // GET: api/CollarData/pet/5
        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IEnumerable<CollarData>>> GetCollarDataByPet(int petId)
        {
            return await _context.CollarData
                .Where(c => c.PetID == petId)
                .OrderByDescending(c => c.Timestamp)
                .ToListAsync();
        }

        // POST: api/CollarData
        [HttpPost]
        public async Task<ActionResult<CollarData>> CreateCollarData(CollarData collarData)
        {
            collarData.Timestamp = DateTime.Now;
            _context.CollarData.Add(collarData);
            await _context.SaveChangesAsync();

            // 檢查異常並產生警報
            var alerts = await _alertService.CheckCollarDataAlertsAsync(collarData);

            return CreatedAtAction(nameof(GetCollarData), new { id = collarData.ID }, new { collarData, alerts });
        }
    }
}
