using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IBookingRepository
{
    Task<int> CreateAsync(int tenantId, CreateBookingRequest request);
    Task<IEnumerable<BookingResponse>> GetByTenantIdAsync(int tenantId);
    Task<BookingResponse?> GetByIdAsync(int id);
    Task UpdateStatusAsync(int id, string status);
}