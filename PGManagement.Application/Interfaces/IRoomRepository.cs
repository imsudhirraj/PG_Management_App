using PGManagement.Application.DTOs;
using PGManagement.Domain.Entities;

namespace PGManagement.Application.Interfaces;

public interface IRoomRepository
{
    Task<int> CreateAsync(Room room);
    Task<IEnumerable<RoomResponse>> GetByPGIdAsync(int pgId);
}