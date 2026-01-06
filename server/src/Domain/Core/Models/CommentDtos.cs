using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

/// <summary>
/// Comment data transfer object for responses
/// </summary>
public record CommentDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int? ParentCommentId { get; init; }
    public string Body { get; set; }
    public string AuthorFullName { get; set; }
    public DateTime Created { get; init; }
    public string Username { get; set; }
    public CommentType Type { get; set; }
    public CommentDto() { }
}

/// <summary>
/// Create comment request data transfer object
/// </summary>
public record CreateCommentDto(
    [Required(ErrorMessage = "Topic ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Topic ID must be greater than 0")]
    int TopicId, 
    
    [Range(1, int.MaxValue, ErrorMessage = "Parent comment ID must be greater than 0")]
    int? ParentCommentId, 
    
    [Required(ErrorMessage = "Comment body is required")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Comment body must be between 1 and 5000 characters")]
    string Body);

/// <summary>
/// Update comment request data transfer object
/// </summary>
public record UpdateCommentDto(
    [Required(ErrorMessage = "Comment body is required")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Comment body must be between 1 and 5000 characters")]
    string Body);