using AllForAll.Models;
using AutoMapper;
using BusinessLogic.Dto.Feedback;
using BusinessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Implementation;

public class FeedbackService : IFeedbackService
{
    private readonly AllForAllDbContext _dbContext;
    private readonly IMapper _mapper;
    public FeedbackService(AllForAllDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }


    public async Task<int> CreateFeedbackAsync(FeedbackRequestDto feedback, CancellationToken cancellation = default)
    {
        
        var mappedFeedback = _mapper.Map<Feedback>(feedback);

        var createdFeedback = await _dbContext.Feedbacks.AddAsync(mappedFeedback, cancellation);

        await _dbContext.SaveChangesAsync(cancellation);

        return createdFeedback.Entity.FeedbackId;
    }

    public async Task DeleteFeedbackAsync(int id, CancellationToken cancellation = default)
    {
        var feedbackToDelete = await _dbContext.Feedbacks.FindAsync(id, cancellation);
        if (feedbackToDelete != null)
        {
            _dbContext.Feedbacks.Remove(feedbackToDelete);
            await _dbContext.SaveChangesAsync(cancellation);
        }
    }

    public async Task<ICollection<Feedback>> GetAllFeedbacksAsync(CancellationToken cancellation = default)
    {
        return await _dbContext.Feedbacks.Include(f => f.User).ToListAsync(cancellation);
    }

    public async Task<Feedback> GetFeedbackByIdAsync(int id, CancellationToken cancellation = default)
    {
        return await _dbContext.Feedbacks.Include(f => f.User).FirstOrDefaultAsync(f => f.FeedbackId == id, cancellation);
    }

    public async Task<bool> IsFeedbackExistAsync(int id, CancellationToken cancellation = default)
    {
        return await _dbContext.Feedbacks.AnyAsync(f => f.FeedbackId == id, cancellation);
    }

    public async Task UpdateFeedbackAsync(int id, FeedbackRequestDto feedback, CancellationToken cancellation = default)
    {
        var feedbackToUpdate = await _dbContext.
            Feedbacks.FirstOrDefaultAsync(f => f.FeedbackId == id, cancellation);
        if (feedbackToUpdate != null)
        {
            _mapper.Map(feedback, feedbackToUpdate);
            _dbContext.Update(feedbackToUpdate);
            await _dbContext.SaveChangesAsync(cancellation);
        }

    }
}