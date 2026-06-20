using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IComplaintRepository
{
    Task<int> CreateAsync(int tenantId, RaiseComplaintRequest request);
    Task<IEnumerable<ComplaintResponse>> GetByTenantIdAsync(int tenantId);
    Task UpdateStatusAsync(int id, string status);
}