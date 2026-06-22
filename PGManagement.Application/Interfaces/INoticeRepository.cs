using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface INoticeRepository
{
    Task<int> CreateAsync(CreateNoticeRequest request);
    Task<IEnumerable<NoticeResponse>> GetByPGIdAsync(int pgId);
    Task<IEnumerable<NoticeOverviewResponse>> GetByOwnerIdAsync(string ownerId);

}