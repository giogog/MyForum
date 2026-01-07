namespace Domain.Entities;

public class Upvote
{
    public int Id { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    public int TopicId { get; set; }

    public Topic Topic { get; set; } = null!;

    [System.ComponentModel.DataAnnotations.Required]
    public int UserId { get; set; }

    public User User { get; set; } = null!;
}
