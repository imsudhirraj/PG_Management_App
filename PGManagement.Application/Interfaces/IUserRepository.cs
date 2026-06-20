using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<int> RegisterAsync(string email, string passwordHash, string fullName, string role);
    Task<UserRecord?> GetByEmailAsync(string email);
}