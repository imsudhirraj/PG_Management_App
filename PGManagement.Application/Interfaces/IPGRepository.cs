using PGManagement.Application.DTOs;
using PGManagement.Domain.Entities;

namespace PGManagement.Application.Interfaces;

public interface IPGRepository
{
    Task<int> CreateAsync(PG pg);

    Task UpdateAsync(int id, CreatePGRequest request);

    Task SoftDeleteAsync(int id);

    Task<PGResponse?> GetByIdAsync(int id);

    Task<IEnumerable<PGSearchResult>> SearchByLocationAsync(
        decimal lat,
        decimal lng,
        double radiusKm);

    Task<IEnumerable<PG>> GetByOwnerIdAsync(string ownerId);
    Task<IEnumerable<PGCardResponse>> SearchPGsAsync(string searchText);
}