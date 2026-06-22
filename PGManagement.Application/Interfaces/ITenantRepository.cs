using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface ITenantRepository
{
    Task<int> CreateAsync(string userId, CreateTenantRequest request);
    Task<TenantResponse?> GetByUserIdAsync(string userId);
    Task<IEnumerable<TenantWithRoomResponse>> GetByOwnerIdAsync(string ownerId);
    Task UpdateAsync(int id, UpdateTenantRequest request);
    Task CheckoutAsync(int tenantId);
}