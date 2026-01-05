namespace Contracts;

public interface IRepositoryManager : IDisposable
{
    IUserRepository UserRepository { get; }
    ICommentRepository CommentRepository { get; }
    ITopicRepository TopicRepository { get; }
    IUpvoteRepository UpvoteRepository { get; }
    IForumRepository ForumRepository { get; }
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task SaveAsync();
}
