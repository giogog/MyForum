using Contracts;
using Domain.Entities;
using Infrastructure.DataConnection;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repository;

public class TopicRepository(ApplicationDataContext context) : BaseRepository<Topic>(context), ITopicRepository
{
    public async Task AddTopictAsync(Topic topic) => Create(topic);

    public async Task DeleteTopicAsync(Topic topic) => Delete(topic);

    public async Task UpdateTopicAsync(Topic topic) => Update(topic);

    public async Task<IEnumerable<Topic>> GetAllTopicWithContentAsync() =>
        await FindAll()
            .AsNoTracking() // Read-only query
            .Include(t=>t.Comments)
                .ThenInclude(c=>c.User)
            .Include(t=>t.User)
            .ToArrayAsync();
            
    public IQueryable<Topic> Topics() => FindAll();

    public async Task<Topic> GetTopicByIdAsync(int id) => 
        await FindByCondition(t => t.Id == id)
            .SingleOrDefaultAsync(); // Don't use AsNoTracking here - may be used for updates

    public async Task<Topic> GetTopicWithContentByIdAsync(int id) =>
        await FindByCondition(t => t.Id == id)
            .AsNoTracking() // Read-only query with includes
            .Include(t => t.User)
            .Include(t=>t.Upvotes)
            .SingleOrDefaultAsync();

    public async Task<IEnumerable<Topic>> GetTopicByUserIdAsync(int userId) => 
        await FindByCondition(t=>t.UserId == userId)
            .AsNoTracking() // Read-only query
            .ToArrayAsync();

    public async Task<IEnumerable<Topic>> GetAllTopicAsyncWithConditionAsync(Expression<Func<Topic, bool>> expression) => 
        await FindByCondition(expression)
            .AsNoTracking() // Read-only query
            .ToArrayAsync();
}
