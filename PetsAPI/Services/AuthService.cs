using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetsAPI.Data;
using PetsAPI.Models;

namespace PetsAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly PetsDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(PetsDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, string? Token)> RegisterAsync(
            string username, string email, string password, string? fullName = null)
        {
            try
            {
                // 檢查用戶名是否已存在
                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    return (false, "用戶名已存在", null);
                }

                // 檢查郵箱是否已存在
                if (await _context.Users.AnyAsync(u => u.Email == email))
                {
                    return (false, "郵箱已被使用", null);
                }

                // 驗證密碼強度
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    return (false, "密碼至少需要6個字符", null);
                }

                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    FullName = fullName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var token = GenerateJwtToken(user);
                _logger.LogInformation($"用戶註冊成功: {username}");
                return (true, "註冊成功", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"註冊異常 - 用戶: {username}, 郵箱: {email}, 錯誤: {ex.Message}");
                return (false, $"註冊失敗: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, string? Token)> LoginAsync(
            string username, string password)
        {
            try
            {
                _logger.LogInformation($"嘗試登入用戶: {username}");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning($"登入失敗：用戶不存在或密碼錯誤 - {username}");
                    return (false, "用戶名或密碼錯誤", null);
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation($"登入成功: {username}");
                return (true, "登入成功", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"登入異常 - 用戶: {username}, 錯誤: {ex.Message}, InnerException: {ex.InnerException?.Message}");
                return (false, $"登入失敗: {ex.Message}", null);
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Pets)
                .FirstOrDefaultAsync(u => u.ID == userId);
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
