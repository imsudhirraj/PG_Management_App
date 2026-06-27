using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IOwnerPaymentSettingsRepository
{
    Task SaveAsync(
        string ownerId,
        OwnerPaymentSettingsRequest request);

    Task<OwnerPaymentSettingsResponse?> GetAsync(
        string ownerId);
}