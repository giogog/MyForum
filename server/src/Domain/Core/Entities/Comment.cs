using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Comment
{
    public int Id { get; set; }

    [Required]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 5000 characters.")]
    public string Body { get; set; } = null!;

    [Required]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    // Nullable for top-level comments that are directly related to a topic
    public int? ParentCommentId { get; set; }

    [ForeignKey("ParentCommentId")]
    public Comment? ParentComment { get; set; }

    // Collection for replies (self-referencing)
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    // Foreign key for the associated topic
    [Required]
    public int TopicId { get; set; }

    public Topic Topic { get; set; } = null!;

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [EnumDataType(typeof(CommentType))]
    public CommentType Type { get; set; }
}