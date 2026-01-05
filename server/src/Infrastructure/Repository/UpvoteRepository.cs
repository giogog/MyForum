using Contracts;
using Domain.Entities;
using Infrastructure.DataConnection;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repository;

public class UpvoteRepository(ApplicationDataContext dataContext) : BaseRepository<Upvote>(dataContext), IUpvoteRepository
{
    public async Task AddUpvoteAsync(Upvote upvote) => Create(upvote);

    public async Task DeleteUpvoteAsync(Upvote upvote) => Delete(upvote);

    public async Task<Upvote> GetUpvote(Expression<Func<Upvote, bool>> expression) => await FindByCondition(expression).FirstOrDefaultAsync();

    public async Task<IEnumerable<Upvote>> GetUpvotesByTopicIdAsync(int topicId) => await FindByCondition(u=>u.TopicId == topicId).ToArrayAsync();

    public async Task<IEnumerable<Upvote>> GetUpvotesByUserIdAsync(int userId) => await FindByCondition(u => u.UserId == userId).ToArrayAsync();
}
