using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

/// <summary>
/// Forum data transfer object for responses
/// </summary>
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

/// <summary>
/// Create forum request data transfer object
/// </summary>
public record CreateForumDto(
    [Required(ErrorMessage = "Forum title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-_.,!?()]+$", ErrorMessage = "Title contains invalid characters")]
    string Title);

/// <summary>
/// Update forum request data transfer object
/// </summary>
public record UpdateForumDto(
    [Required(ErrorMessage = "Forum title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-_.,!?()]+$", ErrorMessage = "Title contains invalid characters")]
    string Title);
