
namespace Contracts;

public interface IServiceManager
{
    IAuthorizationService AuthorizationService { get; }
    ITopicService TopicService { get; }
    IUserService UserService { get; }
    IEmailService EmailService { get; }
    ICommentService CommentService { get; }
    IUpvoteService UpvoteService { get; }
    IForumService ForumService { get; }

}
