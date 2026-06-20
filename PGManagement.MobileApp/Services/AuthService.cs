using PGManagement.MobileApp.Models;

namespace PGManagement.MobileApp.Services;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private readonly ISecureStorageService _secureStorage;

    public AuthService(IApiService apiService, ISecureStorageService secureStorage)
    {
        _apiService = apiService;
        _secureStorage = secureStorage;
    }

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _apiService.PostAsync<LoginRequest, AuthResponse>(
                "Auth/login", new LoginRequest { Email = email, Password = password });

            if (response is null) return (false, "Invalid email or password.");

            await _secureStorage.SaveTokenAsync(response.Token);
            await _secureStorage.SaveUserInfoAsync(response.UserId, response.FullName, response.Role);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Login failed: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string fullName, string role)
    {
        try
        {
            var result = await _apiService.PostRawAsync("Auth/register",
                new RegisterRequest { Email = email, Password = password, FullName = fullName, Role = role });

            return result.IsSuccessStatusCode ? (true, null) : (false, "Registration failed. Email may already be in use.");
        }
        catch (Exception ex)
        {
            return (false, $"Registration failed: {ex.Message}");
        }
    }

    public async Task LogoutAsync() => await _secureStorage.ClearAsync();

    public async Task<bool> IsLoggedInAsync()
    {
        var token = await _secureStorage.GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetCurrentRoleAsync()
    {
        var info = await _secureStorage.GetUserInfoAsync();
        return info?.Role;
    }
}