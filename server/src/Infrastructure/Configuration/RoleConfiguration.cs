using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;


public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("AspNetRoles");

        builder.HasMany(UserRole => UserRole.Users)
            .WithOne(user => user.Role)
            .HasForeignKey(user => user.RoleId)
            .IsRequired();


    }
}
