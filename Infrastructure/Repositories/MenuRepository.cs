using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        public MenuRepository(AppDbContext context) : base(context) { }

        // Lấy menu theo slug
        public async Task<Menu?> GetBySlugAsync(string slug, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(menu => !menu.IsDeleted && menu.Slug == slug)
                .FirstOrDefaultAsync(ct);
        }

        // Lấy toàn bộ liên kết MenuNews theo menuId
        public Task<List<MenuNews>> GetMenuNewsByMenuIdAsync(int menuId, CancellationToken ct = default)
        {
            return _context.MenuNews.Where(mn => mn.MenuId == menuId).ToListAsync(ct);
        }

        // Lấy toàn bộ liên kết MenuNews theo nhiều menuId (batch)
        public Task<List<MenuNews>> GetMenuNewsByMenuIdsAsync(IEnumerable<int> menuIds, CancellationToken ct = default)
        {
            return _context.MenuNews.Where(mn => menuIds.Contains(mn.MenuId)).ToListAsync(ct);
        }

        // Thêm các liên kết MenuNews, tự động gán AssignedAt
        public void AddMenuNewsRange(IEnumerable<MenuNews> menuNewsList)
        {
            var now = DateTime.UtcNow;
            foreach (var menuNews in menuNewsList)
                menuNews.AssignedAt = now;
            _context.MenuNews.AddRange(menuNewsList);
        }

        // Xóa các liên kết MenuNews
        public void RemoveMenuNewsRange(IEnumerable<MenuNews> menuNewsList)
        {
            _context.MenuNews.RemoveRange(menuNewsList);
        }
    }
}