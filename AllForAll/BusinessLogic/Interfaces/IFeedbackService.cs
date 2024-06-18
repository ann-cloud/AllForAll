using AllForAll.Models;
using BusinessLogic.Dto.Feedback;

namespace BusinessLogic.Interfaces;

public interface IFeedbackService
{
    Task<ICollection<Feedback>> GetAllFeedbacksAsync(CancellationToken cancellation = default);
    Task<Feedback> GetFeedbackByIdAsync(int id, CancellationToken cancellation = default);

    Task<bool> IsFeedbackExistAsync(int id, CancellationToken cancellation = default);

    Task<int> CreateFeedbackAsync(FeedbackRequestDto feedback, CancellationToken cancellation = default);

    Task UpdateFeedbackAsync(int id, FeedbackRequestDto feedback, CancellationToken cancellation = default);

    Task DeleteFeedbackAsync(int id, CancellationToken cancellation = default);
}