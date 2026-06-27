using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IKycRepository
{
    Task<int> CreateAsync(int tenantId, int bookingId, string documentType, string fileUrl);
    Task<IEnumerable<KycDocumentResponse>> GetByTenantIdAsync(int tenantId);
    Task UpdateStatusAsync( int id, string status, string ownerId, string? rejectionReason);
    Task<IEnumerable<KycDocumentResponse>> GetPendingByOwnerIdAsync(string ownerId);
    Task<KycDocumentResponse?> GetByIdAsync(int id);

}