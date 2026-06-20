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

            // Composite primary key: 1 cặp (MenuId, NewsId) chỉ tồn tại 1 lần
            builder.HasKey(mn => new { mn.MenuId, mn.NewsId });

            builder.Property(mn => mn.MenuId).HasColumnName("menu_id");
            builder.Property(mn => mn.NewsId).HasColumnName("news_id");
            builder.Property(mn => mn.AssignedAt).HasColumnName("assigned_at");

            // "Khóa ngoại giả" - khai báo quan hệ MenuId -> Menu để EF hiểu navigation
            // (cho phép viết m.MenuNewsList.Select(...) ngay trong code, không cần Include)
            // nhưng KHÔNG bắt SQL Server tạo FK constraint thật khi migrate.
            // DB-first: DB hiện tại không có FK, giữ nguyên đúng yêu cầu, code tự xử lý validate.
            var menuFk = builder.HasOne(mn => mn.Menu)
                   .WithMany(m => m.MenuNewsList)
                   .HasForeignKey(mn => mn.MenuId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .Metadata;
            menuFk.SetIsForeignKeyConstraintCreationDisabled(true);

            var newsFk = builder.HasOne(mn => mn.News)
                   .WithMany(n => n.MenuNewsList)
                   .HasForeignKey(mn => mn.NewsId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .Metadata;
            newsFk.SetIsForeignKeyConstraintCreationDisabled(true);
        }
    }
}
