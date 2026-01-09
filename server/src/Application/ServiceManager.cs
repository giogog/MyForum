using Application.Services;
using AutoMapper;
using Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthorizationService> _authorizationService;
    private readonly Lazy<ITopicService> _topicService;
    private readonly Lazy<IUserService> _userService;
    private readonly Lazy<ICommentService> _commentService;
    private readonly Lazy<IUpvoteService> _upvoteService;
    private readonly Lazy<IForumService> _forumService;
    public ServiceManager(
        IRepositoryManager repositoryManager,
        ITokenGenerator tokenGenerator, 
        IMapper mapper,
        IConfiguration configuration,
        ILoggerFactory loggerFactory

        )
    {
        _authorizationService = new (() => new AuthorizationService(tokenGenerator, repositoryManager, loggerFactory.CreateLogger<AuthorizationService>()));
        _topicService = new(() => new TopicService(repositoryManager, mapper, configuration, loggerFactory.CreateLogger<TopicService>()));
        _userService = new(() => new UserService(repositoryManager, mapper, loggerFactory.CreateLogger<UserService>(),configuration));
        _commentService = new(() => new CommentService(repositoryManager, mapper, configuration, loggerFactory.CreateLogger<CommentService>()));
        _upvoteService = new(() => new UpvoteService(repositoryManager, loggerFactory.CreateLogger<UpvoteService>()));
        _forumService = new(() => new ForumService(repositoryManager, mapper, configuration, loggerFactory.CreateLogger<ForumService>()));
    }
    public IAuthorizationService AuthorizationService => _authorizationService.Value;
    public ITopicService TopicService => _topicService.Value;
    public IUserService UserService => _userService.Value;
    public ICommentService CommentService => _commentService.Value;
    public IUpvoteService UpvoteService => _upvoteService.Value;
    public IForumService ForumService => _forumService.Value;
}
