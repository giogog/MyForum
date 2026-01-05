using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.DataConnection;


public class ApplicationDataContext : IdentityDbContext<User, Role, int
    , IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>
    , IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options)
    {

    }


    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        builder.SeedRoles();
        builder.SeedUsers();
        builder.SeedUserRoles();

        builder.ApplyConfigurationsFromAssembly(
            typeof(Configuration.RoleConfiguration).Assembly
        );


    }
}
