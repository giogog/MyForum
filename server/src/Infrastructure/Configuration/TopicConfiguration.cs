using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable(nameof(Topic));
        builder.HasKey(t => t.Id);

        builder.HasMany(t=>t.Comments)
            .WithOne()
            .HasForeignKey(c=>c.TopicId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(u => u.Upvotes)
            .WithOne(t => t.Topic)
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
