using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class ForumConfiguration : IEntityTypeConfiguration<Forum>
{
    public void Configure(EntityTypeBuilder<Forum> builder)
    {
        builder.ToTable(nameof(Forum));
        builder.HasKey(t => t.Id);

        builder.Property(f => f.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Created)
            .IsRequired();

        builder.HasIndex(f => f.Created);
        builder.HasIndex(f => f.UserId);

        builder.HasMany(f=>f.Topics)
            .WithOne(t=>t.Forum)
            .HasForeignKey(t => t.ForumId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
