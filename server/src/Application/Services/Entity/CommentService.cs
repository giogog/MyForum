using AutoMapper;
using Contracts;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly int _pageSize;
        private readonly ILogger<CommentService> _logger;

        public CommentService(IRepositoryManager repositoryManager, IMapper mapper, IConfiguration configuration, ILogger<CommentService> logger)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _pageSize = Int32.Parse(configuration["ApiSettings:CommentPageSize"]!);
            _logger = logger;
        }

        public async Task CreateComment(int userId, CreateCommentDto commentDto)
        {
            try
            {
                _logger.LogInformation("Creating comment for Topic {TopicId} by User {UserId}", commentDto.TopicId, userId);
                await _repositoryManager.BeginTransactionAsync();

                var topic = await _repositoryManager.TopicRepository.GetTopicByIdAsync(commentDto.TopicId);
                if (topic == null)
                {
                    throw new NotFoundException("Topic Not Found");
                }

                if (topic.Status == Status.Inactive)
                {
                    throw new RestrictedException("You can't comment on this post");
                }

                if (commentDto.ParentCommentId.HasValue)
                {
                    var parentComment = await _repositoryManager.CommentRepository.GetCommentByIdAsync(commentDto.ParentCommentId.Value);
                    if (parentComment == null)
                    {
                        throw new NotFoundException("Parent Comment Not Found");
                    }
                }

                var comment = _mapper.Map<Comment>(commentDto);
                comment.Type = commentDto.ParentCommentId.HasValue ? CommentType.Reply : CommentType.Comment;
                comment.UserId = userId;

                await _repositoryManager.CommentRepository.AddCommentAsync(comment);
                await _repositoryManager.SaveAsync();
                await _repositoryManager.CommitTransactionAsync();
                _logger.LogInformation("Successfully created comment for Topic {TopicId} by User {UserId}", commentDto.TopicId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating comment for Topic {TopicId} by User {UserId}", commentDto.TopicId, userId);
                await _repositoryManager.RollbackTransactionAsync();
                throw; 
            }
        }

        public async Task UpdateComment(int userId, int commentId, UpdateCommentDto commentDto)
        {
            try
            {
                _logger.LogInformation("Updating comment {CommentId} by User {UserId}", commentId, userId);
                await _repositoryManager.BeginTransactionAsync();

                var comment = await _repositoryManager.CommentRepository.GetCommentByIdAsync(commentId);
                if (comment == null)
                {
                    throw new NotFoundException("Comment Not Found");
                }
                if (comment.UserId != userId)
                {
                    throw new RestrictedException("You can't update this comment");
                }

                comment.Body = commentDto.Body;
                await _repositoryManager.CommentRepository.UpdateCommentAsync(comment);
                await _repositoryManager.SaveAsync();
                await _repositoryManager.CommitTransactionAsync();
                _logger.LogInformation("Successfully updated comment {CommentId} by User {UserId}", commentId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating comment {CommentId} by User {UserId}", commentId, userId);
                await _repositoryManager.RollbackTransactionAsync();
                throw; 
            }
        }

        public async Task DeleteComment(int userId, int commentId)
        {
            try
            {
                _logger.LogInformation("Deleting comment {CommentId} by User {UserId}", commentId, userId);
                await _repositoryManager.BeginTransactionAsync();

                var comment = await _repositoryManager.CommentRepository.GetCommentByIdAsync(commentId);
                if (comment == null)
                {
                    throw new NotFoundException("Comment Not Found");
                }
                if (comment.UserId != userId)
                {
                    throw new RestrictedException("You can't delete this comment");
                }

                await _repositoryManager.CommentRepository.DeleteCommentAsync(comment);
                await _repositoryManager.SaveAsync();
                await _repositoryManager.CommitTransactionAsync();
                _logger.LogInformation("Successfully deleted comment {CommentId} by User {UserId}", commentId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting comment {CommentId} by User {UserId}", commentId, userId);
                await _repositoryManager.RollbackTransactionAsync();
                throw; // Re-throw the exception to propagate it to the caller
            }
        }

        public async Task<PagedList<CommentDto>> GetCommentByPage(int page, int topicId)
        {
            _logger.LogInformation("Getting comments for Topic {TopicId}, page {Page}", topicId, page);
            
            var comments = _repositoryManager.CommentRepository.Comments()
                .AsNoTracking() // Read-only query optimization
                .Where(c => c.TopicId == topicId)
                .Select(c => new CommentDto
                {
                    Body = c.Body,
                    Id = c.Id,
                    ParentCommentId = c.ParentCommentId,
                    Created = c.Created,
                    Type = c.Type,
                    UserId = c.UserId,
                    Username = c.User.UserName!,
                    AuthorFullName = $"{c.User.Name} {c.User.Surname}"
                })
                .OrderByDescending(c => c.Created);

            return await PagedList<CommentDto>.CreateAsync(comments, page, _pageSize);
        }

        public async Task<IEnumerable<CommentDto>> GetAllComment(int topicId)
        {
            _logger.LogInformation("Getting all comments for Topic {TopicId}", topicId);
            
            var comments = await _repositoryManager.CommentRepository.Comments()
                .AsNoTracking() // Read-only query optimization
                .Where(c => c.TopicId == topicId)
                .Select(c => new CommentDto
                {
                    Body = c.Body,
                    Id = c.Id,
                    ParentCommentId = c.ParentCommentId,
                    Created = c.Created,
                    Type = c.Type,
                    UserId = c.UserId,
                    Username = c.User.UserName!,
                    AuthorFullName = $"{c.User.Name} {c.User.Surname}"
                })
                .OrderByDescending(c => c.Created)
                .ToListAsync();
                
            if (!comments.Any())
                throw new NotFoundException("Comments on this topic not found");

            return comments;
        }
    }
}
