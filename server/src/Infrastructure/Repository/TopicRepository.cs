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
        .Include(t=>t.Comments)
            .ThenInclude(c=>c.User)
        .Include(t=>t.User)
        .ToArrayAsync();
    public IQueryable<Topic> Topics() => FindAll();

    public async Task<Topic> GetTopicByIdAsync(int id) => 
        await FindByCondition(t => t.Id == id).FirstOrDefaultAsync();

    public async Task<Topic> GetTopicWithContentByIdAsync(int id) =>
    await FindByCondition(t => t.Id == id)
        .Include(t => t.User)
        .Include(t=>t.Upvotes)
        .FirstOrDefaultAsync();

    public async Task<IEnumerable<Topic>> GetTopicByUserIdAsync(int userId) => 
        await FindByCondition(t=>t.UserId == userId).ToArrayAsync();

    public async Task<IEnumerable<Topic>> GetAllTopicAsyncWithConditionAsync(Expression<Func<Topic, bool>> expression) => 
        await FindByCondition(expression).ToArrayAsync();
}
