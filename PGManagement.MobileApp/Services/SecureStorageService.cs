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
            await SafeGetAsync(TokenKey);

        public async Task SaveUserInfoAsync(int userId, string fullName, string role)
        {
            await SecureStorage.Default.SetAsync(UserIdKey, userId.ToString());
            await SecureStorage.Default.SetAsync(FullNameKey, fullName);
            await SecureStorage.Default.SetAsync(RoleKey, role);
        }

        public async Task<(int UserId, string FullName, string Role)?> GetUserInfoAsync()
        {
            var userId = await SafeGetAsync(UserIdKey);
            var fullName = await SafeGetAsync(FullNameKey);
            var role = await SafeGetAsync(RoleKey);

            if (userId is null || fullName is null || role is null) return null;

            // Defensive: SafeGetAsync already clears corrupt entries, but a
            // malformed (non-numeric) userId should never bubble up as an
            // unhandled FormatException either.
            if (!int.TryParse(userId, out var parsedUserId)) return null;

            return (parsedUserId, fullName, role);
        }

        public Task ClearAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            SecureStorage.Default.Remove(UserIdKey);
            SecureStorage.Default.Remove(FullNameKey);
            SecureStorage.Default.Remove(RoleKey);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Reads a value from SecureStorage, self-healing from Android
        /// Keystore decryption failures (javax.crypto.BadPaddingException,
        /// wrapped by MAUI as an ArgumentException/generic Exception on
        /// GetAsync). This happens when the encrypted SharedPreferences
        /// entry survives an app reinstall/re-sign but the Keystore key that
        /// encrypted it does not — the ciphertext can never be decrypted
        /// again. Without this guard, that failure surfaces as an unrelated
        /// crash/error anywhere the token is read (e.g. a "Search Failed"
        /// dialog on the home page instead of a normal logged-out state).
        /// </summary>
        private static async Task<string?> SafeGetAsync(string key)
        {
            try
            {
                return await SecureStorage.Default.GetAsync(key);
            }
            catch
            {
                // Corrupted/undecryptable entry for this key — remove it so
                // it can't keep throwing on every future read, and treat it
                // as "not present" so callers (IsLoggedInAsync, etc.) fall
                // back to a clean logged-out state instead of crashing.
                try
                {
                    SecureStorage.Default.Remove(key);
                }
                catch
                {
                    // Best-effort cleanup; ignore if removal itself fails.
                }

                return null;
            }
        }
    }
}