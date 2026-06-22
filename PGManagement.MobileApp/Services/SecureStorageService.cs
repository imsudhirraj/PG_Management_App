using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGManagement.MobileApp.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        private const string TokenKey = "auth_token";
        private const string UserIdKey = "user_id";
        private const string FullNameKey = "full_name";
        private const string RoleKey = "user_role";

        public async Task SaveTokenAsync(string token) =>
            await SecureStorage.Default.SetAsync(TokenKey, token);

        public async Task<string?> GetTokenAsync() =>
            await SecureStorage.Default.GetAsync(TokenKey);

        public async Task SaveUserInfoAsync(int userId, string fullName, string role)
        {
            await SecureStorage.Default.SetAsync(UserIdKey, userId.ToString());
            await SecureStorage.Default.SetAsync(FullNameKey, fullName);
            await SecureStorage.Default.SetAsync(RoleKey, role);
        }

        public async Task<(int UserId, string FullName, string Role)?> GetUserInfoAsync()
        {
            var userId = await SecureStorage.Default.GetAsync(UserIdKey);
            var fullName = await SecureStorage.Default.GetAsync(FullNameKey);
            var role = await SecureStorage.Default.GetAsync(RoleKey);

            if (userId is null || fullName is null || role is null) return null;
            return (int.Parse(userId), fullName, role);
        }

        public Task ClearAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            SecureStorage.Default.Remove(UserIdKey);
            SecureStorage.Default.Remove(FullNameKey);
            SecureStorage.Default.Remove(RoleKey);
            return Task.CompletedTask;
        }
    }
}
