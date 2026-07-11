using System.Data;
using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;

namespace PGManagement.Infrastructure.Repositories;

public class RecommendationRepository : IRecommendationRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RecommendationRepository(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Premium / Featured PGs
    /// </summary>
    public async Task<IEnumerable<PGCardResponse>> GetFeaturedAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<PGCardResponse>(
            "dbo.sp_Recommendation_GetFeatured",
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// Nearby PGs
    /// </summary>
    public async Task<IEnumerable<PGCardResponse>> GetNearbyAsync(
        decimal latitude,
        decimal longitude,
        double radiusKm)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<PGCardResponse>(
            "dbo.sp_Recommendation_GetNearby",
            new
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm
            },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// Personalized recommendation
    /// </summary>
    public async Task<IEnumerable<PGCardResponse>> GetRecommendedAsync(
        string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<PGCardResponse>(
            "dbo.sp_Recommendation_GetRecommended",
            new
            {
                UserId = userId
            },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// Recently added PGs
    /// </summary>
    public async Task<IEnumerable<PGCardResponse>> GetRecentAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<PGCardResponse>(
            "dbo.sp_Recommendation_GetRecent",
            commandType: CommandType.StoredProcedure);
    }
}