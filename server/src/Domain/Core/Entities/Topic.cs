using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;

namespace Domain.Entities;

public class Topic
{
    public int Id { get; set; }

    [Required]
    [StringLength(300, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 300 characters.")]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(10000, MinimumLength = 10, ErrorMessage = "Body must be between 10 and 10000 characters.")]
    public string Body { get; set; } = null!;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public int CommentNum => Comments?.Count ?? 0;
    [EnumDataType(typeof(State))]
    public State State { get; set; } = State.Pending;

    [EnumDataType(typeof(Status))]
    public Status Status { get; set; } = Status.Active;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public int UpvotesNum => Upvotes?.Count ?? 0;
    public ICollection<Upvote> Upvotes { get; set; } = new List<Upvote>();

    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Required]
    public int ForumId { get; set; }
    public Forum Forum { get; set; } = null!;

}