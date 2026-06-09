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
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menus");
            builder.Property(m => m.Id)
                   .HasColumnName("menu_id")
                   .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(m => m.Slug)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(m => m.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(m => m.CreatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(m => m.UpdatedAt)
                   .HasDefaultValueSql("SYSUTCDATETIME()");
            builder.HasQueryFilter(m => !m.IsDeleted);
            builder.HasMany(m => m.MenuNews)
                   .WithOne(mn => mn.Menu)
                   .HasForeignKey(mn => mn.MenuId)
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
