using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IRoomRepository
{
    Task<int> CreateAsync(CreateRoomRequest request);
    Task<IEnumerable<RoomResponse>> GetByPGIdAsync(int pgId);
}