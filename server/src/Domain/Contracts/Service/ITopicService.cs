using Domain.Models;

namespace Contracts;

public interface ITopicService
{
    Task<TopicDto> GetSingleTopic(int topicId);
    Task<PagedList<TopicDto>> GetAllTopicsByPage(int page);
    Task<PagedList<TopicDto>> GetForumTopicsByPage(int forumId,int page);
    Task<PagedList<TopicDto>> GetTopicsWithUserIdByPage(int userId, int page);
    Task<PagedList<TopicDto>> GetPendingTopicsByPage(int page);
    Task CreateTopic(int userId,CreateTopicDto createTopicDto);
    Task UpdateTopic(int topicId, UpdateTopicDto updateTopicDto);
    Task DeleteTopic(int topicId);
    Task ChangeTopicStatus(int topicId, Status status);
    Task ChangeTopicState(int topicId, State state);
}
