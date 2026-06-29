using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menus");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("menu_id");
            builder.Property(m => m.Name).HasColumnName("name");
            builder.Property(m => m.Slug).HasColumnName("slug");
            builder.Property(m => m.DisplayOrder).HasColumnName("display_order");
            builder.Property(m => m.IsDeleted).HasColumnName("is_deleted");
            builder.Property(m => m.CreatedAt).HasColumnName("created_at");
            builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");
        }
    }
}