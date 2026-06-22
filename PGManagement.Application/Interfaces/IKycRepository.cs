using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IKycRepository
{
    Task<int> CreateAsync(int tenantId, string documentType, string fileUrl);
    Task<IEnumerable<KycDocumentResponse>> GetByTenantIdAsync(int tenantId);
}