using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<int>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public Ban Banned { get; set; } = Ban.NotBanned;
    public ICollection<Forum> Forums { get; set; }
    public ICollection<Topic> Topics { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<UserRole> Roles { get; set; }
    public ICollection<Upvote> Upvotes { get; set; }
}
