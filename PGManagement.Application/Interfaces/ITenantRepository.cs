using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface ITenantRepository
{
    Task<int> CreateAsync(string userId, CreateTenantRequest request);
    Task<TenantResponse?> GetByUserIdAsync(string userId);
}