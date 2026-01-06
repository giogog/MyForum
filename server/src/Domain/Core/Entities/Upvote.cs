namespace Domain.Entities;

public class Upvote
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public Topic Topic { get; set; }
    public int UserId { get; set; }
}
