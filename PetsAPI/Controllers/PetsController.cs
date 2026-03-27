using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models;

namespace PetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PetsController : ControllerBase
    {
        private readonly PetsDbContext _context;

        public PetsController(PetsDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return -1;
            }
            return userId;
        }

        // GET: api/Pets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPets()
        {
            var userId = GetCurrentUserId();
            if (userId == -1)
            {
                return Unauthorized();
            }

            try
            {
                var pets = await _context.Pets
                    .Where(p => p.UserID == userId)
                    .Select(p => new
                    {
                        p.ID,
                        p.Name,
                        p.Species,
                        p.Birthday,
                        p.WeightGoal,
                        p.UserID
                    })
                    .ToListAsync();
                
                return Ok(pets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        // GET: api/Pets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPet(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == -1)
            {
                return Unauthorized();
            }

            try
            {
                var pet = await _context.Pets
                    .Where(p => p.ID == id && p.UserID == userId)
                    .Select(p => new
                    {
                        p.ID,
                        p.Name,
                        p.Species,
                        p.Birthday,
                        p.WeightGoal,
                        p.UserID
                    })
                    .FirstOrDefaultAsync();

                if (pet == null)
                {
                    return NotFound();
                }
                
                return Ok(pet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/Pets
        [HttpPost]
        public async Task<ActionResult<Pet>> CreatePet(Pet pet)
        {
            var userId = GetCurrentUserId();
            if (userId == -1)
            {
                return Unauthorized();
            }

            pet.UserID = userId;
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPet), new { id = pet.ID }, pet);
        }

        // PUT: api/Pets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePet(int id, Pet pet)
        {
            var userId = GetCurrentUserId();
            if (userId == -1)
            {
                return Unauthorized();
            }

            if (id != pet.ID)
            {
                return BadRequest();
            }

            var existingPet = await _context.Pets.FindAsync(id);
            if (existingPet == null || existingPet.UserID != userId)
            {
                return NotFound();
            }

            pet.UserID = userId;
            _context.Entry(existingPet).State = EntityState.Detached;
            _context.Entry(pet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Pets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == -1)
            {
                return Unauthorized();
            }

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null || pet.UserID != userId)
            {
                return NotFound();
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.ID == id);
        }
    }
}
