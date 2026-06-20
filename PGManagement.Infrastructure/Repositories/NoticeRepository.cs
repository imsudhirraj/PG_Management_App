using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class NoticeRepository : INoticeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public NoticeRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateAsync(CreateNoticeRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Notice_Insert",
            new { request.PGId, request.Title, request.Message },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<NoticeResponse>> GetByPGIdAsync(int pgId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<NoticeResponse>(
            "dbo.sp_Notice_GetByPGId",
            new { PGId = pgId },
            commandType: CommandType.StoredProcedure);
    }
}