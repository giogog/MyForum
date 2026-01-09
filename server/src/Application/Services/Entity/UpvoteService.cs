using Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class UpvoteService : IUpvoteService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<UpvoteService> _logger;

        public UpvoteService(IRepositoryManager repositoryManager, ILogger<UpvoteService> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        public async Task Upvote(int userId, int topicId)
        {
            try
            {
                _logger.LogInformation("Starting transaction for user {UserId} upvoting topic {TopicId}", userId, topicId);
                await _repositoryManager.BeginTransactionAsync();

                var upvoted = await _repositoryManager.UpvoteRepository.GetUpvote(u => u.UserId == userId && u.TopicId == topicId);
                if (upvoted != null)
                {
                    _logger.LogWarning("User {UserId} has already upvoted Topic {TopicId}", userId, topicId);
                    await _repositoryManager.UpvoteRepository.DeleteUpvoteAsync(upvoted);
                }
                else
                {
                    await _repositoryManager.UpvoteRepository.AddUpvoteAsync(new Upvote { UserId = userId, TopicId = topicId });
                }

                await _repositoryManager.SaveAsync();
                await _repositoryManager.CommitTransactionAsync();
                _logger.LogInformation("Transaction committed for user {UserId} upvoting topic {TopicId}", userId, topicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upvoting Topic {TopicId} by User {UserId}", topicId, userId);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
