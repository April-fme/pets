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
            // 測試用假資料（資料庫設定好後請改回資料庫查詢）
            var mockPets = new List<Pet>
            {
                new Pet
                {
                    ID = 1,
                    Name = "小黃",
                    Species = "狗",
                    Birthday = new DateTime(2020, 5, 15),
                    WeightGoal = 8.5m
                },
                new Pet
                {
                    ID = 2,
                    Name = "咪咪",
                    Species = "貓",
                    Birthday = new DateTime(2021, 3, 20),
                    WeightGoal = 4.2m
                }
            };
            
            return Ok(mockPets);
            
            // 資料庫版本（資料庫連接後取消註解）
            // return await _context.Pets.ToListAsync();
        }

        // GET: api/Pets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPet(int id)
        {
            // 測試用假資料
            var mockPets = new List<Pet>
            {
                new Pet { ID = 1, Name = "小黃", Species = "狗", Birthday = new DateTime(2020, 5, 15), WeightGoal = 8.5m },
                new Pet { ID = 2, Name = "咪咪", Species = "貓", Birthday = new DateTime(2021, 3, 20), WeightGoal = 4.2m }
            };
            
            var pet = mockPets.FirstOrDefault(p => p.ID == id);
            if (pet == null)
            {
                return NotFound();
            }
            return pet;
            
            // 資料庫版本（資料庫連接後取消註解）
            // var pet = await _context.Pets.FindAsync(id);
            // if (pet == null)
            // {
            //     return NotFound();
            // }
            // return pet;
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
