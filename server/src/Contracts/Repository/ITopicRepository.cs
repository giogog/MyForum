using Domain.Entities;
using System.Linq.Expressions;

namespace Contracts;

public interface ITopicRepository
{
    Task AddTopictAsync(Topic topic);
    Task DeleteTopicAsync(Topic topic);
    Task UpdateTopicAsync(Topic topic);
    Task<Topic> GetTopicByIdAsync(int id);
    Task<Topic> GetTopicWithContentByIdAsync(int id);
    Task<IEnumerable<Topic>> GetTopicByUserIdAsync(int userId);
    Task<IEnumerable<Topic>> GetAllTopicAsyncWithConditionAsync(Expression<Func<Topic, bool>> expression);
    Task<IEnumerable<Topic>> GetAllTopicWithContentAsync();
    IQueryable<Topic> Topics();
}
