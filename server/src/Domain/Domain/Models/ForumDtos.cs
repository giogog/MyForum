namespace Domain.Models;

public record ForumDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Title { get; init; }
    public int TopicNum { get; init; }
    public DateTime Created { get; init; }
    public string Username { get; init; }
    public State State { get; init; }
    public Status Status { get; init; }
    public ForumDto() { }
}


public record CreateForumDto(string Title);

public record UpdateForumDto(string Title);
