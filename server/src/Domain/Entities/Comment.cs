using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Comment
{
    public int Id { get; set; }

    [Required]
    [StringLength(1000, ErrorMessage = "Content must be between 1 and 1000 characters.")]
    public string Body { get; set; }

    [Required]
    public DateTime Created { get; set; } = DateTime.Now;

    // Nullable for top-level comments that are directly related to a topic
    public int? ParentCommentId { get; set; }

    [ForeignKey("ParentCommentId")]
    public Comment ParentComment { get; set; }

    // Collection for replies (self-referencing)
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    // Foreign key for the associated topic
    public int TopicId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [EnumDataType(typeof(CommentType))]
    public CommentType Type { get; set; }
}