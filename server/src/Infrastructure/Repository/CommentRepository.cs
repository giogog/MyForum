using Contracts;
using Domain.Entities;
using Infrastructure.DataConnection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class CommentRepository(ApplicationDataContext context) : BaseRepository<Comment>(context), ICommentRepository
{
    public async Task AddCommentAsync(Comment comment) => Create(comment);

    public async Task DeleteCommentAsync(Comment comment) => Delete(comment);
    
    public async Task UpdateCommentAsync(Comment comment) => Update(comment);

    public async Task<Comment> GetCommentByIdAsync(int id) => 
        await FindByCondition(c => c.Id == id)
            .SingleOrDefaultAsync(); // Don't use AsNoTracking - may be used for updates

    public async Task<IEnumerable<Comment>> GetCommentsByTopicIdAsync(int topicId) => 
        await FindByCondition(c => c.TopicId == topicId)
            .AsNoTracking() // Read-only query
            .ToArrayAsync();

    public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId) => 
        await FindByCondition(c => c.UserId == userId)
            .AsNoTracking() // Read-only query
            .ToArrayAsync();

    public IQueryable<Comment> Comments() => FindAll();
}
