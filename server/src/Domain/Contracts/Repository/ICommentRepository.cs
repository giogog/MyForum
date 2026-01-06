using Domain.Entities;

namespace Contracts;

public interface ICommentRepository
{
    Task AddCommentAsync(Comment comment);
    Task DeleteCommentAsync(Comment comment);
    Task UpdateCommentAsync(Comment comment);
    Task<Comment> GetCommentByIdAsync(int id);
    Task<IEnumerable<Comment>> GetCommentsByTopicIdAsync(int topicId);
    Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId);
    IQueryable<Comment> Comments();

}
