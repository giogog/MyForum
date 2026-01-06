using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

/// <summary>
/// User data transfer object for responses
/// </summary>
public record UserDto
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}

/// <summary>
/// Authorized user data transfer object with sensitive information
/// </summary>
public record AuthorizedUserDto(
    int Id,
    
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    string Name, 
    
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    string Surname, 
    
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    string Username, 
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    string Email,
    
    Ban Banned, 
    string[] Roles);





