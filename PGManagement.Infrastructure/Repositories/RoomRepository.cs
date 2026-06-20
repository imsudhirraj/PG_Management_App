using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public RoomRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateAsync(CreateRoomRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Room_Insert",
            new { request.PGId, request.RoomNumber, request.RoomType, request.TotalBeds, request.RentAmount },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<RoomResponse>> GetByPGIdAsync(int pgId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<RoomResponse>(
            "dbo.sp_Room_GetByPGId",
            new { PGId = pgId },
            commandType: CommandType.StoredProcedure);
    }
}