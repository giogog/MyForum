using Domain.Entities;

namespace Contracts;

public interface IForumRepository
{
    Task AddForumAsync(Forum forum);
    Task DeleteForumAsync(Forum forum);
    Task UpdateForumAsync(Forum forum);
    Task<IEnumerable<Forum>> GetForums();
    Task<Forum> GetForumByIdAsync(int id);
    IQueryable<Forum> Forums();
}
