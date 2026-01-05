using Domain.Models;

namespace Contracts;

public interface ICommentService
{
    Task CreateComment(int UserId,CreateCommentDto commentDto);
    Task UpdateComment(int userId, int commentId, UpdateCommentDto commentDto);
    Task DeleteComment(int userId, int commentId);
    Task<PagedList<CommentDto>> GetCommentByPage(int page,int topicId);
    Task<IEnumerable<CommentDto>> GetAllComment(int topicId);
}
