using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.MobileApp.Services;

public interface ISecureStorageService
{
    Task SaveTokenAsync(string token);
    Task<string?> GetTokenAsync();
    Task SaveUserInfoAsync(int userId, string fullName, string role);
    Task<(int UserId, string FullName, string Role)?> GetUserInfoAsync();
    Task ClearAsync();
}


