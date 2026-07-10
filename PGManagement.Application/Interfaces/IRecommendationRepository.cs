using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IRecommendationRepository
{
    Task<IEnumerable<PGCardResponse>> GetFeaturedAsync();

    Task<IEnumerable<PGCardResponse>> GetNearbyAsync(
        decimal latitude,
        decimal longitude,
        double radiusKm);

    Task<IEnumerable<PGCardResponse>> GetRecommendedAsync(string userId);

    Task<IEnumerable<PGCardResponse>> GetRecentAsync();
}