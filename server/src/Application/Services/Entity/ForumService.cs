using AutoMapper;
using Contracts;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services; 
public class ForumService:IForumService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IMapper _mapper;
    private readonly int _pageSize;
    private readonly ILogger<ForumService> _logger;

    public ForumService(IRepositoryManager repositoryManager, IMapper mapper, IConfiguration configuration, ILogger<ForumService> logger)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _pageSize = Int32.Parse(configuration["ApiSettings:PageSize"]);
        _logger = logger;

    }
    public async Task<PagedList<ForumDto>> GetPendingForums(int page)
    {
        var forums = _repositoryManager.ForumRepository.Forums()
            .AsNoTracking() // Read-only query optimization
            .Where(f => f.State == State.Pending)
            .Select(f => new ForumDto
            {
                Id = f.Id,
                Created = f.Created,
                State = f.State,
                Status = f.Status,
                Title = f.Title,
                TopicNum = f.TopicNum,
                UserId = f.UserId,
                Username = f.User.UserName
            })
            .OrderByDescending(f => f.Created);

        return await PagedList<ForumDto>.CreateAsync(forums, page, _pageSize);
    }
    public async Task<PagedList<ForumDto>> GetDeletedForums(int page)
    {
        var forums = _repositoryManager.ForumRepository.Forums()
            .AsNoTracking() // Read-only query optimization
            .Where(f => f.Status == Status.Deleted)
            .Select(f => new ForumDto
            {
                Id = f.Id,
                Created = f.Created,
                State = f.State,
                Status = f.Status,
                Title = f.Title,
                TopicNum = f.TopicNum,
                UserId = f.UserId,
                Username = f.User.UserName
            })
            .OrderByDescending(f => f.State)
            .ThenByDescending(f => f.Created);

        return await PagedList<ForumDto>.CreateAsync(forums, page, _pageSize);
    }
    public async Task<PagedList<ForumDto>> GetAllForumsByPage(int page)
    {
        var forums = _repositoryManager.ForumRepository.Forums()
            .AsNoTracking() // Read-only query optimization
            .Where(t => t.State != State.Pending)
            .Select(f => new ForumDto
            {
                Id = f.Id,
                Created = f.Created,
                State = f.State,
                Status = f.Status,
                Title = f.Title,
                TopicNum = f.TopicNum,
                UserId = f.UserId,
                Username = f.User.UserName
            })
            .OrderByDescending(f=>f.State)
            .ThenByDescending(f => f.Created);

        return await PagedList<ForumDto>.CreateAsync(forums, page, _pageSize);
    }
    public async Task<PagedList<ForumDto>> GetForumsByPage(int page)
    {
        var forums = _repositoryManager.ForumRepository.Forums()
            .AsNoTracking() // Read-only query optimization
            .Where(f=>f.State == State.Show && f.Status != Status.Deleted)
            .Select(f => new ForumDto
            {
                Id = f.Id,
                Created = f.Created,
                State = f.State,
                Status = f.Status,
                Title = f.Title,
                TopicNum = f.TopicNum,
                UserId = f.UserId,
                Username = f.User.UserName
            })
            .OrderByDescending(f =>f.Created);


        return await PagedList<ForumDto>.CreateAsync(forums, page,_pageSize);
    }
    public async Task CreateForum(int userId, CreateForumDto createForumDto)
    {
        _logger.LogInformation("Creating forum by User {UserId}", userId);
        var forum = _mapper.Map<Forum>(createForumDto);
        forum.UserId = userId;

        _repositoryManager.ForumRepository.AddForumAsync(forum);
        await _repositoryManager.SaveAsync();

        _logger.LogInformation("Forum created successfully by User {UserId}", userId);
    }

    public async Task DeleteForumFromUser(int forumId)
    {
        _logger.LogInformation("Deleting forum {ForumId} from User", forumId);

        await _repositoryManager.BeginTransactionAsync();

        try
        {
            var forum = await _repositoryManager.ForumRepository.GetForumByIdAsync(forumId);
            if (forum == null)
            {
                _logger.LogWarning("Forum {ForumId} not found", forumId);
                throw new NotFoundException("Topic not found");
            }
            forum.Status = Status.Deleted;
            await _repositoryManager.ForumRepository.UpdateForumAsync(forum);
            await _repositoryManager.SaveAsync();

            await _repositoryManager.CommitTransactionAsync();

            _logger.LogInformation("Forum {ForumId} deleted successfully", forumId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting Forum {ForumId}", forumId);
            await _repositoryManager.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task DeleteForum(int forumId)
    {
        _logger.LogInformation("Deleting forum {ForumId}", forumId);

        await _repositoryManager.BeginTransactionAsync();

        try
        {
            var topic = await _repositoryManager.ForumRepository.GetForumByIdAsync(forumId);
            if (topic == null)
            {
                _logger.LogWarning("Forum {ForumId} not found", forumId);
                throw new NotFoundException("Topic not found");
            }

            await _repositoryManager.ForumRepository.DeleteForumAsync(topic);
            await _repositoryManager.SaveAsync();

            await _repositoryManager.CommitTransactionAsync();

            _logger.LogInformation("Forum {ForumId} deleted successfully", forumId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting Forum {ForumId}", forumId);
            await _repositoryManager.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task UpdateForum(int forumId, UpdateForumDto updateForumDto)
    {
        _logger.LogInformation("Updating forum {ForumId}", forumId);

        await _repositoryManager.BeginTransactionAsync();

        try
        {
            var forum = await _repositoryManager.ForumRepository.GetForumByIdAsync(forumId);
            if (forum == null)
            {
                _logger.LogWarning("Forum {ForumId} not found", forumId);
                throw new NotFoundException("Forum not found");
            }
            forum.State = State.Pending;
            forum.Title = updateForumDto.Title;

            await _repositoryManager.ForumRepository.UpdateForumAsync(forum);
            await _repositoryManager.SaveAsync();

            await _repositoryManager.CommitTransactionAsync();

            _logger.LogInformation("Forum {ForumId} updated successfully", forumId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating forum {ForumId}", forumId);
            await _repositoryManager.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task ChangeForumState(int forumId, State state)
    {
        _logger.LogInformation("Changing state of forum {ForumId} to {State}", forumId, state);

        await _repositoryManager.BeginTransactionAsync();

        try
        {
            var forum = await _repositoryManager.ForumRepository.GetForumByIdAsync(forumId);
            if (forum == null)
            {
                _logger.LogWarning("Forum {ForumId} not found", forumId);
                throw new NotFoundException("Forum not found");
            }
            
            forum.State = state;

            await _repositoryManager.ForumRepository.UpdateForumAsync(forum);
            await _repositoryManager.SaveAsync();

            await _repositoryManager.CommitTransactionAsync();

            _logger.LogInformation("State of forum {ForumId} changed to {State}", forumId, state);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while changing state of forum {ForumId}", forumId);
            await _repositoryManager.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task ChangeForumStatus(int forumId, Status status)
    {
        _logger.LogInformation("Changing status of forum {ForumId} to {Status}", forumId, status);

        await _repositoryManager.BeginTransactionAsync();
        
        try
        {
            var forum = await _repositoryManager.ForumRepository.GetForumByIdAsync(forumId);
            if (forum == null)
            {
                _logger.LogWarning("Forum {ForumId} not found", forumId);
                throw new NotFoundException("Forum not found");
            }

            forum.Status = status;

            await _repositoryManager.ForumRepository.UpdateForumAsync(forum);
            await _repositoryManager.SaveAsync();

            await _repositoryManager.CommitTransactionAsync();

            _logger.LogInformation("Status of forum {ForumId} changed to {Status}", forumId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while changing status of forum {ForumId}", forumId);
            await _repositoryManager.RollbackTransactionAsync();
            throw;
        }
    }


}
