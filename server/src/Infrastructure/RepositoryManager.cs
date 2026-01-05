using Contracts;
using Domain.Entities;
using Infrastructure.DataConnection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly ApplicationDataContext _context;
    private IDbContextTransaction _transaction;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<ICommentRepository> _commentRepository;
    private readonly Lazy<ITopicRepository> _topicRepository;
    private readonly Lazy<IUpvoteRepository> _upvoteRepository;
    private readonly Lazy<IForumRepository> _forumRepository;
    public RepositoryManager(ApplicationDataContext context,UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _context = context;
        _userRepository = new(() => new UserRepository(userManager, roleManager));
        _commentRepository = new(() => new CommentRepository(context));
        _topicRepository = new(() => new TopicRepository(context));
        _upvoteRepository = new(() => new UpvoteRepository(context));
        _forumRepository = new(() => new ForumRepository(context));
    }
    public IUserRepository UserRepository => _userRepository.Value;
    public ICommentRepository CommentRepository => _commentRepository.Value;    
    public ITopicRepository TopicRepository => _topicRepository.Value;  
    public IUpvoteRepository UpvoteRepository => _upvoteRepository.Value;
    public IForumRepository ForumRepository => _forumRepository.Value;
    public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();
    public async Task CommitTransactionAsync() => await _transaction.CommitAsync();
    public async Task RollbackTransactionAsync() => await _transaction.RollbackAsync();
    public void Dispose() => _transaction?.Dispose();
    public Task SaveAsync() => _context.SaveChangesAsync();
}
