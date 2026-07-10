using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReviewRepository(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(
        string userId,
        CreateReviewRequest request)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Review_Insert",
            new
            {
                UserId = userId,
                request.PGId,
                request.Rating,
                request.Review
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<ReviewResponse>>
        GetByPGIdAsync(int pgId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QueryAsync<ReviewResponse>(
            "dbo.sp_Review_GetByPGId",
            new
            {
                PGId = pgId
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task ReplyAsync(
        int reviewId,
        string reply)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.sp_Review_Reply",
            new
            {
                ReviewId = reviewId,
                Reply = reply
            },
            commandType: CommandType.StoredProcedure);
    }
}