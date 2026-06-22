using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IDashboardRepository
{
    Task<OwnerDashboardStats> GetOwnerStatsAsync(string ownerId);
}