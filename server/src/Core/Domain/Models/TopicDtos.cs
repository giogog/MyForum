using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

/// <summary>
/// Topic data transfer object for responses
/// </summary>
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

/// <summary>
/// Create topic request data transfer object
/// </summary>
public record CreateTopicDto(
    [Required(ErrorMessage = "Forum ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Forum ID must be greater than 0")]
    int ForumId,
    
    [Required(ErrorMessage = "Topic title is required")]
    [StringLength(300, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 300 characters")]
    string Title, 
    
    [Required(ErrorMessage = "Topic body is required")]
    [StringLength(10000, MinimumLength = 10, ErrorMessage = "Body must be between 10 and 10000 characters")]
    string Body);

/// <summary>
/// Update topic request data transfer object
/// </summary>
public record UpdateTopicDto(
    [Required(ErrorMessage = "Topic title is required")]
    [StringLength(300, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 300 characters")]
    string Title, 
    
    [Required(ErrorMessage = "Topic body is required")]
    [StringLength(10000, MinimumLength = 10, ErrorMessage = "Body must be between 10 and 10000 characters")]
    string Body);

