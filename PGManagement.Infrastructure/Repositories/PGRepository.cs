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
            pg.Longitude
        };
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_PG_Insert", parameters, commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<PGResponse?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<PGResponse>(
            "dbo.sp_PG_GetById", new { Id = id }, commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<PGSearchResult>> SearchByLocationAsync(decimal lat, decimal lng, double radiusKm)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<PGSearchResult>(
            "dbo.sp_PG_SearchByLocation",
            new { UserLat = lat, UserLong = lng, RadiusKm = radiusKm },
            commandType: System.Data.CommandType.StoredProcedure);
    }
}