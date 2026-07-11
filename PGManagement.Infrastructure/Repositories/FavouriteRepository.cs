using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class FavouriteRepository : IFavouriteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public FavouriteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(string userId, int pgId)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.sp_Wishlist_Add",
            new
            {
                UserId = userId,
                PGId = pgId
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task RemoveAsync(string userId, int pgId)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.sp_Wishlist_Remove",
            new
            {
                UserId = userId,
                PGId = pgId
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExistsAsync(string userId, int pgId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<bool>(
            "dbo.sp_Wishlist_Exists",
            new
            {
                UserId = userId,
                PGId = pgId
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<PGSearchResult>> GetMyFavouritesAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<PGSearchResult>(
            "dbo.sp_Wishlist_GetByUserId",
            new
            {
                UserId = userId
            },
            commandType: CommandType.StoredProcedure);
    }
}