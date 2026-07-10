using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IRecentlyViewedRepository
{
    Task AddAsync(string userId, int pgId);

    Task<IEnumerable<RecentlyViewedResponse>> GetByUserAsync(string userId);
}