using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.ToTable("News");

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
                   .HasColumnName("news_id")
                   .ValueGeneratedOnAdd();

            builder.Property(n => n.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            builder.Property(n => n.Content).HasColumnName("content").IsRequired();
            builder.Property(n => n.Summary).HasColumnName("summary").HasMaxLength(1000);
            builder.Property(n => n.IsPublished).HasColumnName("is_published");
            builder.Property(n => n.IsDeleted).HasColumnName("is_deleted");
            builder.Property(n => n.CreatedAt).HasColumnName("created_at");
            builder.Property(n => n.UpdatedAt).HasColumnName("updated_at");
        }
    }
}