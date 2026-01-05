using Domain.Models;

namespace Contracts;

public interface IForumService
{
    Task<PagedList<ForumDto>> GetPendingForums(int page);
    Task<PagedList<ForumDto>> GetAllForumsByPage(int page);
    Task<PagedList<ForumDto>> GetForumsByPage(int page);
    Task<PagedList<ForumDto>> GetDeletedForums(int page);
    Task CreateForum(int userId, CreateForumDto createForumDto);
    Task DeleteForum(int forumId);
    Task DeleteForumFromUser(int forumId);
    Task UpdateForum(int forumId, UpdateForumDto updateForumDto);
    Task ChangeForumState(int forumId, State state);
    Task ChangeForumStatus(int forumId, Status status);


}
