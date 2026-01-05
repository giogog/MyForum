using Contracts;
using Domain.Entities;
using Infrastructure.DataConnection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repository;

/// <summary>
/// Repository manager implementing Unit of Work pattern with transaction support
/// </summary>
public class RepositoryManager : IRepositoryManager, IAsyncDisposable
{
    private readonly ApplicationDataContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<ICommentRepository> _commentRepository;
    private readonly Lazy<ITopicRepository> _topicRepository;
    private readonly Lazy<IUpvoteRepository> _upvoteRepository;
    private readonly Lazy<IForumRepository> _forumRepository;
    
    public RepositoryManager(ApplicationDataContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
    
    public async Task BeginTransactionAsync() 
    {
        if (_transaction != null)
            throw new InvalidOperationException("Transaction already started");
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync() 
    {
        if (_transaction == null)
            throw new InvalidOperationException("No transaction to commit");
        try
        {
            await _transaction.CommitAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public async Task RollbackTransactionAsync() 
    {
        if (_transaction == null)
            throw new InvalidOperationException("No transaction to rollback");
        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public void Dispose() 
    {
        _transaction?.Dispose();
        GC.SuppressFinalize(this);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
    
    public Task SaveAsync() => _context.SaveChangesAsync();
}
