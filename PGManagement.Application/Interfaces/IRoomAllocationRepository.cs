using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IRoomAllocationRepository
{
    Task<int> AllocateAsync(AllocateRoomRequest request);
    Task VacateAsync(int allocationId);
    Task<IEnumerable<AllocationOverviewResponse>> GetByPGIdAsync(int pgId);

}