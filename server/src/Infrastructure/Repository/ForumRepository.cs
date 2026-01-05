using Contracts;
using Domain.Entities;
using Infrastructure.DataConnection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class ForumRepository(ApplicationDataContext dataContext) : BaseRepository<Forum>(dataContext), IForumRepository
{
    public async Task AddForumAsync(Forum forum) => Create(forum);

    public async Task DeleteForumAsync(Forum forum) => Delete(forum);

    public IQueryable<Forum> Forums() => FindAll();
    public async Task<IEnumerable<Forum>> GetForums() => await FindAll().ToArrayAsync();
    public async Task<Forum> GetForumByIdAsync(int id) => await FindByCondition(f=>f.Id==id).FirstOrDefaultAsync();

    public async Task UpdateForumAsync(Forum forum) => Update(forum);
}
