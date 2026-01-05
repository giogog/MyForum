namespace Contracts;

public interface IUpvoteService
{
    Task Upvote(int UserId, int TopicId);

}
