using PGManagement.MobileApp.Models;

namespace PGManagement.MobileApp.Services;

public interface IAuthService
{
    Task<(bool Success, string? Error)> LoginAsync(string email, string password);
    Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string fullName, string role);
    Task LogoutAsync();
    Task<bool> IsLoggedInAsync();
    Task<string?> GetCurrentRoleAsync();
}