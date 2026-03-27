using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsAPI.Services;

namespace PetsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Email) || 
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("用戶名、郵箱和密碼不能為空");
            }

            var (success, message, token) = await _authService.RegisterAsync(
                request.Username, 
                request.Email, 
                request.Password, 
                request.FullName);

            if (success)
            {
                return Ok(new { message, token });
            }

            return BadRequest(message);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("用戶名和密碼不能為空");
            }

            var (success, message, token) = await _authService.LoginAsync(
                request.Username, 
                request.Password);

            if (success)
            {
                return Ok(new { message, token });
            }

            return Unauthorized(message);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("無效的用戶ID");
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("用戶未找到");
            }

            return Ok(new
            {
                id = user.ID,
                username = user.Username,
                email = user.Email,
                fullName = user.FullName,
                createdAt = user.CreatedAt,
                pets = user.Pets.Select(p => new
                {
                    id = p.ID,
                    name = p.Name,
                    species = p.Species,
                    birthday = p.Birthday,
                    weightGoal = p.WeightGoal
                }).ToList()
            });
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
