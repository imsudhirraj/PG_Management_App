using PGManagement.Application.DTOs;

namespace PGManagement.Application.Interfaces;

public interface IReviewRepository
{
    Task<int> CreateAsync(
        string userId,
        CreateReviewRequest request);

    Task<IEnumerable<ReviewResponse>> GetByPGIdAsync(
        int pgId);

    Task ReplyAsync(
        int reviewId,
        string reply);
}