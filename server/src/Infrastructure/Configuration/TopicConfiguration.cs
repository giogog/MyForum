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

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(t => t.Body)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(t => t.Created)
            .IsRequired();

        builder.HasIndex(t => t.ForumId);
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.Created);
    }
}
