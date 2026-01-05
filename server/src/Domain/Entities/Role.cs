using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Role : IdentityRole<int>
{
    public ICollection<UserRole> Users { get; set; }
}
