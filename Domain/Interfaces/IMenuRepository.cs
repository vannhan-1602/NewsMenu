using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<Menu?> GetBySlugAsync(string slug, CancellationToken ct = default);

        // Thêm liên kết Menu - News vào bảng trung gian
        Task AddMenuNewsRangeAsync(IEnumerable<MenuNews> menuNews, CancellationToken ct = default);

        // Xóa hết liên kết cũ của 1 Menu (dùng khi Update/Delete)
        Task RemoveMenuNewsByMenuIdAsync(int menuId, CancellationToken ct = default);

        // Lấy danh sách News gắn với 1 Menu 
        Task<List<(int NewsId, string Title, bool IsPublished)>> GetNewsOfMenuAsync(int menuId, CancellationToken ct = default);

        // Lấy theo nhiều MenuId - dùng cho GetList (tránh N+1)
        Task<List<(int MenuId, int NewsId, string Title, bool IsPublished)>> GetNewsOfMenuListAsync(IEnumerable<int> menuIds, CancellationToken ct = default);
    }
}
