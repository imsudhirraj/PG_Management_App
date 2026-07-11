using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;
namespace PGManagement.Infrastructure.Repositories;

public class RecentlyViewedRepository : IRecentlyViewedRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public RecentlyViewedRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task AddAsync(string userId, int pgId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var user_Id = int.Parse(userId);
        await connection.ExecuteAsync(
        "dbo.sp_RecentlyViewed_Add",
        new
        {
            UserId = user_Id,
            PGId = pgId
        },
        commandType: CommandType.StoredProcedure);
    }
    public async Task<IEnumerable<RecentlyViewedResponse>> GetByUserAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var user_Id = int.Parse(userId);
        return await connection.QueryAsync<RecentlyViewedResponse>(
        "dbo.sp_RecentlyViewed_GetByUser",
        new
        {
            UserId = user_Id
        },
        commandType: CommandType.StoredProcedure);
    }
}