namespace Domain.Models;

public record TopicDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int ForumId { get; set; }
    public string Title { get; init; }
    public string Body { get; init; }
    public int? CommentNum { get; init; }
    public int? UpvotesNum { get; init; }
    public DateTime Created { get; init; }
    public string Username { get; init; }
    public string AuthorFullName { get; init; }
    public string? ForumTitle { get; init; }
    public Status Status { get; init; }
    public State State { get; init; }

    // Parameterless constructor
    public TopicDto() { }
}


public record CreateTopicDto(int ForumId,string Title, string Body);

public record UpdateTopicDto(string Title, string Body);

