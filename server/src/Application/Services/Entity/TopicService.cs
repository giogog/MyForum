using AutoMapper;
using Contracts;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Application.Services
{
    public class TopicService : ITopicService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly int _pageSize;
        private readonly ILogger<TopicService> _logger;

        public TopicService(IRepositoryManager repositoryManager, IMapper mapper, IConfiguration configuration, ILogger<TopicService> logger)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _pageSize = Int32.Parse(configuration["ApiSettings:PageSize"]);
            _logger = logger;
        }
        public async Task<PagedList<TopicDto>> GetAllTopicsByPage(int page)
        {
            _logger.LogInformation("Fetching all topics for page {Page}", page);

            var topicDtos = _repositoryManager.TopicRepository.Topics()
                .Where(t=>t.State != State.Pending)
                .Include(t => t.User)
                .Include(t => t.Upvotes)
                .Select(t => new TopicDto
                {
                    Title = t.Title,
                    UserId = t.UserId,
                    ForumId = t.ForumId,
                    Body = t.Body,
                    Username = t.User.UserName,
                    AuthorFullName = $"{t.User.Name} {t.User.Surname}",
                    UpvotesNum = t.UpvotesNum,
                    Created = t.Created,
                    Id = t.Id,
                    Status = t.Status,
                    State = t.State

                })
                .OrderByDescending(t => t.Status == Status.Active);

            return await PagedList<TopicDto>.CreateAsync(topicDtos, page, _pageSize);
        }
        public async Task<PagedList<TopicDto>> GetForumTopicsByPage(int forumId, int page)
        {
            _logger.LogInformation("Fetching topics for page {Page}", page);

            var topicDtos = _repositoryManager.TopicRepository.Topics()
                .Where(t => t.ForumId == forumId && t.State == State.Show)
                .Include(t => t.Comments)
                .Include(t => t.User)
                .Include(t => t.Upvotes)
                .Select(t => new TopicDto
                {
                    Title = t.Title,
                    UserId = t.UserId,
                    ForumId = t.ForumId,
                    Body = t.Body,
                    Username = t.User.UserName,
                    AuthorFullName = $"{t.User.Name} {t.User.Surname}",
                    CommentNum = t.CommentNum,
                    UpvotesNum = t.UpvotesNum,
                    Created = t.Created,
                    Id = t.Id,
                    Status = t.Status,
                    State = t.State

                })
                .OrderByDescending(t => t.Status == Status.Active);

            return await PagedList<TopicDto>.CreateAsync(topicDtos, page, _pageSize);
        }
        public async Task<PagedList<TopicDto>> GetTopicsWithUserIdByPage(int userId,int page)
        {
            _logger.LogInformation("Fetching pending topics for page {Page}", page);

            var topicDtos = _repositoryManager.TopicRepository.Topics()
                .Where(t => t.UserId == userId)
                .Include(t => t.Forum)
                .Include(t => t.User)
                .Include(t => t.Upvotes)
                .Select(t => new TopicDto
                {
                    Title = t.Title,
                    UserId = t.UserId,
                    ForumId = t.ForumId,
                    Body = t.Body,
                    Username = t.User.UserName,
                    AuthorFullName = $"{t.User.Name} {t.User.Surname}",
                    ForumTitle = t.Forum.Title,
                    UpvotesNum=t.UpvotesNum,
                    Created = t.Created,
                    Id = t.Id,
                    Status = t.Status,
                    State = t.State
                })
                .OrderByDescending(t => t.Status == Status.Active);

            return await PagedList<TopicDto>.CreateAsync(topicDtos, page, _pageSize);
        }
        public async Task<PagedList<TopicDto>> GetPendingTopicsByPage(int page)
        {
            _logger.LogInformation("Fetching pending topics for page {Page}", page);

            var topicDtos = _repositoryManager.TopicRepository.Topics()
                .Where(t => t.State == State.Pending)
                .Include(t => t.Forum)
                .Include(t => t.User)
                .Select(t => new TopicDto
                {
                    Title = t.Title,
                    UserId = t.UserId,
                    ForumId = t.ForumId,
                    Body = t.Body,
                    Username = t.User.UserName,
                    AuthorFullName = $"{t.User.Name} {t.User.Surname}",
                    ForumTitle = t.Forum.Title,
                    Created = t.Created,
                    Id = t.Id,
                    Status = t.Status,
                    State = t.State
                })
                .OrderByDescending(t => t.Status == Status.Active);

            return await PagedList<TopicDto>.CreateAsync(topicDtos, page, _pageSize);
        }
        public async Task<TopicDto> GetSingleTopic(int topicId)
        {
            _logger.LogInformation("Fetching topic {TopicId} with content", topicId);

            var topic = await _repositoryManager.TopicRepository.GetTopicWithContentByIdAsync(topicId);
            if (topic == null)
            {
                _logger.LogWarning("Topic {TopicId} not found", topicId);
                throw new NotFoundException("Topic not found");
            }

            return _mapper.Map<TopicDto>(topic);
        }
        public async Task CreateTopic(int userId, CreateTopicDto createTopicDto)
        {
            
            try
            {
                await _repositoryManager.BeginTransactionAsync();
                var forum = await _repositoryManager.ForumRepository.GetForumByIdAsync(createTopicDto.ForumId);
                if (forum == null)
                    throw new NotFoundException("Forum not found");
                _logger.LogInformation("Creating topic by User {UserId}", userId);

                var topic = _mapper.Map<Topic>(createTopicDto);
                topic.UserId = userId;

                _repositoryManager.TopicRepository.AddTopictAsync(topic);
                await _repositoryManager.SaveAsync();

                await _repositoryManager.CommitTransactionAsync();

                _logger.LogInformation("Topic created successfully by User {UserId}", userId);

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error occurred while creating Topic");
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }

        }
        public async Task UpdateTopic(int topicId, UpdateTopicDto updateTopicDto)
        {
            _logger.LogInformation("Updating topic {TopicId}", topicId);

            await _repositoryManager.BeginTransactionAsync();

            try
            {
                var topic = await _repositoryManager.TopicRepository.GetTopicByIdAsync(topicId);
                if (topic == null)
                {
                    _logger.LogWarning("Topic {TopicId} not found", topicId);
                    throw new NotFoundException("Topic not found");
                }

                if (topic.Status == Status.Inactive)
                {
                    _logger.LogWarning("Topic {TopicId} is inactive and cannot be updated", topicId);
                    throw new RestrictedException("You can't update this post");
                }

                topic.Body = updateTopicDto.Body;
                topic.Title = updateTopicDto.Title;

                await _repositoryManager.TopicRepository.UpdateTopicAsync(topic);
                await _repositoryManager.SaveAsync();

                await _repositoryManager.CommitTransactionAsync();

                _logger.LogInformation("Topic {TopicId} updated successfully", topicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating topic {TopicId}", topicId);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task DeleteTopic(int topicId)
        {
            _logger.LogInformation("Deleting topic {TopicId}", topicId);

            await _repositoryManager.BeginTransactionAsync();

            try
            {
                var topic = await _repositoryManager.TopicRepository.GetTopicByIdAsync(topicId);
                if (topic == null)
                {
                    _logger.LogWarning("Topic {TopicId} not found", topicId);
                    throw new NotFoundException("Topic not found");
                }

                await _repositoryManager.TopicRepository.DeleteTopicAsync(topic);
                await _repositoryManager.SaveAsync();

                await _repositoryManager.CommitTransactionAsync();

                _logger.LogInformation("Topic {TopicId} deleted successfully", topicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting topic {TopicId}", topicId);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task ChangeTopicStatus(int topicId, Status status)
        {
            _logger.LogInformation("Changing status of topic {TopicId} to {Status}", topicId, status);

            await _repositoryManager.BeginTransactionAsync();

            try
            {
                var topic = await _repositoryManager.TopicRepository.GetTopicByIdAsync(topicId);
                if (topic == null)
                {
                    _logger.LogWarning("Topic {TopicId} not found", topicId);
                    throw new NotFoundException("Topic not found");
                }

                topic.Status = status;

                await _repositoryManager.TopicRepository.UpdateTopicAsync(topic);
                await _repositoryManager.SaveAsync();

                await _repositoryManager.CommitTransactionAsync();

                _logger.LogInformation("Status of topic {TopicId} changed to {Status}", topicId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing status of topic {TopicId}", topicId);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }
        public async Task ChangeTopicState(int topicId, State state)
        {
            _logger.LogInformation("Changing state of topic {TopicId} to {Status}", topicId, state);

            await _repositoryManager.BeginTransactionAsync();

            try
            {
                var topic = await _repositoryManager.TopicRepository.GetTopicByIdAsync(topicId);
                if (topic == null)
                {
                    _logger.LogWarning("Topic {TopicId} not found", topicId);
                    throw new NotFoundException("Topic not found");
                }

                topic.State = state;

                await _repositoryManager.TopicRepository.UpdateTopicAsync(topic);
                await _repositoryManager.SaveAsync();

                await _repositoryManager.CommitTransactionAsync();

                _logger.LogInformation("state of topic {TopicId} changed to {Status}", topicId, state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing state of topic {TopicId}", topicId);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }


    }
}
