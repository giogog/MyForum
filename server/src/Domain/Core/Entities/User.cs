using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User : IdentityUser<int>
{
    [Required]
    [StringLength(50)]
    public required string Name { get; set; }

    [Required]
    [StringLength(50)]
    public required string Surname { get; set; }

    public Ban Banned { get; set; } = Ban.NotBanned;

    public ICollection<Forum> Forums { get; set; } = new List<Forum>();
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    public ICollection<Upvote> Upvotes { get; set; } = new List<Upvote>();
}
