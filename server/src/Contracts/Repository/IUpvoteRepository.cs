using Domain.Entities;
using System.Linq.Expressions;

namespace Contracts;

public interface IUpvoteRepository
{
    Task AddUpvoteAsync(Upvote upvote);
    Task DeleteUpvoteAsync(Upvote upvote);
    Task<Upvote> GetUpvote(Expression<Func<Upvote,bool>> expression);
    Task<IEnumerable<Upvote>> GetUpvotesByTopicIdAsync(int topicId);
    Task<IEnumerable<Upvote>> GetUpvotesByUserIdAsync(int userId);
}
