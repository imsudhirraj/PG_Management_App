using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Domain.Entities;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class PGRepository : IPGRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public PGRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateAsync(PG pg)
    {
        using var connection = _connectionFactory.CreateConnection();
        var parameters = new
        {
            pg.OwnerId,
            pg.Name,
            pg.Address,
            pg.City,
            pg.Description,
            pg.Latitude,
            pg.Longitude,
            pg.CoverImageUrl,
            pg.PropertyType,
            pg.GenderType,
            pg.DepositAmount,
            pg.FoodAvailable,
            pg.WifiAvailable,
            pg.LaundryAvailable,
            pg.ParkingAvailable,
            pg.ACAvailable,
            pg.Featured,
            pg.Verified
        };
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_PG_Insert", parameters, commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task UpdateAsync(int id, CreatePGRequest request)
{
    using var connection = _connectionFactory.CreateConnection();

    await connection.ExecuteAsync(
        "dbo.sp_PG_Update",
        new
        {
            Id = id,

            request.Name,
            request.Address,
            request.City,
            request.Description,

            request.Latitude,
            request.Longitude,

            request.PropertyType,
            request.GenderType,

            request.DepositAmount,

            request.Featured,
            request.Verified,

            request.FoodAvailable,
            request.WifiAvailable,
            request.LaundryAvailable,
            request.ParkingAvailable,
            request.ACAvailable,

            request.CoverImageUrl
        },
        commandType: CommandType.StoredProcedure);
}

    public async Task SoftDeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync("dbo.sp_PG_SoftDelete", new { Id = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<PGResponse?> GetByIdAsync(int id)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        using var multi =
            await connection.QueryMultipleAsync(
                "dbo.sp_PG_GetById",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

        var pg =
            await multi.ReadFirstOrDefaultAsync<PGResponse>();

        if (pg == null)
            return null;

        pg.Images =
            (await multi.ReadAsync<PGImageResponse>())
            .ToList();

        pg.Amenities =
            (await multi.ReadAsync<PGAmenityResponse>())
            .ToList();

        pg.Rooms =
            (await multi.ReadAsync<RoomResponse>())
            .ToList();

        pg.NearbyPlaces =
            (await multi.ReadAsync<PGNearbyPlaceResponse>())
            .ToList();

        pg.Rules =
            (await multi.ReadAsync<PGRuleResponse>())
            .ToList();

        pg.FAQs =
            (await multi.ReadAsync<PGFAQResponse>())
            .ToList();

        pg.Reviews =
            (await multi.ReadAsync<PGReviewResponse>())
            .ToList();

        return pg;
    }

    public async Task<IEnumerable<PGSearchResult>> SearchByLocationAsync(decimal lat, decimal lng, double radiusKm)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<PGSearchResult>(
            "dbo.sp_PG_SearchByLocation",
            new { UserLat = lat, UserLong = lng, RadiusKm = radiusKm },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<PG>> GetByOwnerIdAsync(string ownerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<PG>(
            "dbo.sp_PG_GetByOwnerId", new { OwnerId = ownerId }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<PGCardResponse>> SearchPGsAsync(string searchText)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@SearchText", searchText);

        return await connection.QueryAsync<PGCardResponse>(
            "usp_PG_SearchByText",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}