using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WardConfiguration : IEntityTypeConfiguration<Ward>
    {
        public void Configure(EntityTypeBuilder<Ward> builder)
        {
            builder.ToTable("Wards");

            builder.HasKey(w => w.Id);
            builder.Property(w => w.Id)
                   .HasColumnName("ward_id")
                   .ValueGeneratedOnAdd();

            builder.Property(w => w.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            builder.Property(w => w.ParentId).HasColumnName("parent_id").HasDefaultValue(0);
            builder.Property(w => w.CountryId).HasColumnName("country_id");
            builder.Property(w => w.IsDeleted).HasColumnName("is_deleted");
            builder.Property(w => w.CreatedAt).HasColumnName("created_at");
            builder.Property(w => w.UpdatedAt).HasColumnName("updated_at");

            builder.HasOne(w => w.Parent)
                   .WithMany(w => w.Children)
                   .HasForeignKey(w => w.ParentId)
                   .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(w => w.Country)
                   .WithMany(c => c.Wards)
                   .HasForeignKey(w => w.CountryId)
                   .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
