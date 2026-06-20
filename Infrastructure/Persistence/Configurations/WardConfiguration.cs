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

            // "Khóa ngoại giả" - tự tham chiếu (đệ quy): Ward.parent_id -> Ward.ward_id
            // ParentId = 0 nghĩa là không có cha (Tỉnh/TP) - EF tự không match được Ward nào có ward_id = 0
            // (IDENTITY bắt đầu từ 1), nên Parent sẽ tự nhiên là null, không cần xử lý đặc biệt
            var parentFk = builder.HasOne(w => w.Parent)
                   .WithMany(w => w.Children)
                   .HasForeignKey(w => w.ParentId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .Metadata;
            parentFk.SetIsForeignKeyConstraintCreationDisabled(true);

            // Ward -> Country
            var countryFk = builder.HasOne(w => w.Country)
                   .WithMany(c => c.Wards)
                   .HasForeignKey(w => w.CountryId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .Metadata;
            countryFk.SetIsForeignKeyConstraintCreationDisabled(true);
        }
    }
}
