using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsAPI.Data;
using PetsAPI.Models;

namespace PetsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly PetsDbContext _context;

        public PetsController(PetsDbContext context)
        {
            _context = context;
        }

        // GET: api/Pets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPets()
        {
            return await _context.Pets.ToListAsync();
        }

        // GET: api/Pets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPet(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            return pet;
        }

        // POST: api/Pets
        [HttpPost]
        public async Task<ActionResult<Pet>> CreatePet(Pet pet)
        {
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPet), new { id = pet.ID }, pet);
        }

        // PUT: api/Pets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePet(int id, Pet pet)
        {
            if (id != pet.ID)
            {
                return BadRequest();
            }

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
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
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
