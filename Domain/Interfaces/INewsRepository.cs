using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface INewsRepository : IRepository<News>
    {
        // Thêm liên kết News - Menu vào bảng trung gian
        Task AddMenuNewsRangeAsync(IEnumerable<MenuNews> menuNews, CancellationToken ct = default);

        // Xóa hết liên kết cũ của 1 News (dùng khi Update/ Delete)
        Task RemoveMenuNewsByNewsIdAsync(int newsId, CancellationToken ct = default);

        // Lấy danh sách Menu gắn với 1 News 
        Task<List<(int MenuId, string Name, string Slug, int DisplayOrder)>> GetMenusOfNewsAsync(int newsId, CancellationToken ct = default);

        // Lấy theo nhiều NewsId - dùng cho GetList (tránh N+1)
        Task<List<(int NewsId, int MenuId, string Name, string Slug, int DisplayOrder)>> GetMenusOfNewsListAsync(IEnumerable<int> newsIds, CancellationToken ct = default);
    }
}
