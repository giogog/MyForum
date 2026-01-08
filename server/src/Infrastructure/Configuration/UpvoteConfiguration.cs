using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class UpvoteConfiguration : IEntityTypeConfiguration<Upvote>
{
    public void Configure(EntityTypeBuilder<Upvote> builder)
    {
        builder.ToTable(nameof(Upvote));
        builder.HasKey(u => u.Id);

        builder.HasOne(u => u.Topic)
            .WithMany(t => t.Upvotes)
            .HasForeignKey(u => u.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.User)
            .WithMany(user => user.Upvotes)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enforce one upvote per (user, topic).
        builder.HasIndex(u => new { u.UserId, u.TopicId })
            .IsUnique();

        builder.HasIndex(u => u.TopicId);
        builder.HasIndex(u => u.UserId);
    }
}
