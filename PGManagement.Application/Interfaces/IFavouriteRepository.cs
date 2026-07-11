using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IFavouriteRepository
{
    Task AddAsync(string userId, int pgId);

    Task RemoveAsync(string userId, int pgId);

    Task<bool> ExistsAsync(string userId, int pgId);

    Task<IEnumerable<PGSearchResult>> GetMyFavouritesAsync(string userId);
}