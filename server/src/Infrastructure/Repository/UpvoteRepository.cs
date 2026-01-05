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

    public async Task<Upvote> GetUpvote(Expression<Func<Upvote, bool>> expression) => 
        await FindByCondition(expression)
            .SingleOrDefaultAsync(); // Don't use AsNoTracking - may be used for delete

    public async Task<IEnumerable<Upvote>> GetUpvotesByTopicIdAsync(int topicId) => 
        await FindByCondition(u=>u.TopicId == topicId)
            .AsNoTracking() // Read-only query
            .ToArrayAsync();

    public async Task<IEnumerable<Upvote>> GetUpvotesByUserIdAsync(int userId) => 
        await FindByCondition(u => u.UserId == userId)
            .AsNoTracking() // Read-only query
            .ToArrayAsync();
}
