using Dapper;
using PGManagement.Application.DTOs;
using PGManagement.Application.Interfaces;
using PGManagement.Domain.Entities;
using PGManagement.Infrastructure.Persistence;
using System.Data;

namespace PGManagement.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public RoomRepository(IDbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<int> CreateAsync(Room room)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new
        {
            room.PGId,
            room.RoomNumber,
            room.RoomType,
            room.Floor,
            room.TotalBeds,
            room.RentAmount,
            room.SecurityDeposit,
            room.Description,
            room.Status
        };

        return await connection.ExecuteScalarAsync<int>(
            "dbo.sp_Room_Insert",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<RoomResponse>> GetByPGIdAsync(int pgId)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "dbo.sp_Room_GetByPGId",
            new { PGId = pgId },
            commandType: CommandType.StoredProcedure);

        //---------------------------------------------------------
        // Result Set 1
        //---------------------------------------------------------

        var rooms = (await multi.ReadAsync<RoomResponse>()).ToList();

        //---------------------------------------------------------
        // Result Set 2
        //---------------------------------------------------------

        var images = (await multi.ReadAsync<RoomImageResponse>()).ToList();

        //---------------------------------------------------------
        // Result Set 3
        //---------------------------------------------------------

        var amenities = (await multi.ReadAsync<RoomAmenityResponse>()).ToList();

        //---------------------------------------------------------
        // Populate Images & Amenities
        //---------------------------------------------------------

        foreach (var room in rooms)
        {
            room.Images = images
                .Where(i => i.RoomId == room.Id)
                .OrderBy(i => i.IsCover ? 0 : 1)
                .ToList();

            room.Amenities = amenities
                .Where(a => a.RoomId == room.Id)
                .ToList();
        }

        return rooms;
    }
}