using PetsAPI.Models;

namespace PetsAPI.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, string? Token)> RegisterAsync(string username, string email, string password, string? fullName = null);
        Task<(bool Success, string Message, string? Token)> LoginAsync(string username, string password);
        Task<User?> GetUserByIdAsync(int userId);
        string GenerateJwtToken(User user);
    }
}
