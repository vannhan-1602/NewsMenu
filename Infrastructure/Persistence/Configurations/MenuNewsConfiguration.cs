using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MenuNewsConfiguration : IEntityTypeConfiguration<MenuNews>
    {
        public void Configure(EntityTypeBuilder<MenuNews> builder)
        {
            builder.ToTable("MenuNews");
            builder.HasKey(mn => new { mn.MenuId, mn.NewsId });

            builder.Property(mn => mn.MenuId).HasColumnName("menu_id");
            builder.Property(mn => mn.NewsId).HasColumnName("news_id");
            builder.Property(mn => mn.AssignedAt).HasColumnName("assigned_at");

            builder.HasOne(mn => mn.Menu)
                   .WithMany(m => m.MenuNewsList)
                   .HasForeignKey(mn => mn.MenuId);

            builder.HasOne(mn => mn.News)
                   .WithMany(n => n.MenuNewsList)
                   .HasForeignKey(mn => mn.NewsId);
        }
    }
}