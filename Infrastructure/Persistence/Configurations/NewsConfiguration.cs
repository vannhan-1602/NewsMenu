using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
namespace Infrastructure.Persistence.Configurations
{
    public class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.ToTable("News");
            builder.Property(n => n.Id)
                   .HasColumnName("news_id")
                   .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title).HasMaxLength(500).IsRequired();
            builder.Property(n => n.Summary).HasMaxLength(1000);
            builder.Property(n => n.IsDeleted).HasDefaultValue(false);
            builder.Property(n => n.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(n => n.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasQueryFilter(n => !n.IsDeleted);

            builder.HasMany(n => n.MenuNews)
                   .WithOne(mn => mn.News)
                   .HasForeignKey(mn => mn.NewsId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
