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
            return GetMenuNewsAsync(menuNews => menuNews.MenuId == menuId, ct);
        }

        // Lấy toàn bộ liên kết MenuNews theo nhiều menuId (batch)
        public Task<List<MenuNews>> GetMenuNewsByMenuIdsAsync(IEnumerable<int> menuIds, CancellationToken ct = default)
        {
            return GetMenuNewsAsync(menuNews => menuIds.Contains(menuNews.MenuId), ct);
        }
    }
}