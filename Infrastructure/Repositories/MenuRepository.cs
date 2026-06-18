using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        public MenuRepository(AppDbContext context) : base(context) { }

        public async Task<Menu?> GetBySlugAsync(string slug, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(x => !x.IsDeleted && x.Slug == slug)
                .FirstOrDefaultAsync(ct);
        }

        public async Task AddMenuNewsRangeAsync(IEnumerable<MenuNews> menuNews, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            foreach (var mn in menuNews)
                mn.AssignedAt = now;

            await _context.MenuNews.AddRangeAsync(menuNews, ct);
        }

        public async Task RemoveMenuNewsByMenuIdAsync(int menuId, CancellationToken ct = default)
        {
            var existing = await _context.MenuNews
                .Where(x => x.MenuId == menuId)
                .ToListAsync(ct);

            _context.MenuNews.RemoveRange(existing);
        }

        
        public async Task<List<(int NewsId, string Title, bool IsPublished)>> GetNewsOfMenuAsync(int menuId, CancellationToken ct = default)
        {
            return await (
                from mn in _context.MenuNews
                join n in _context.News on mn.NewsId equals n.Id
                where mn.MenuId == menuId && !n.IsDeleted
                select new ValueTuple<int, string, bool>(n.Id, n.Title, n.IsPublished)
            ).ToListAsync(ct);
        }

        // 1 query cho cả page - tránh N+1
        public async Task<List<(int MenuId, int NewsId, string Title, bool IsPublished)>> GetNewsOfMenuListAsync(IEnumerable<int> menuIds, CancellationToken ct = default)
        {
            var idList = menuIds.Distinct().ToList();
            return await (
                from mn in _context.MenuNews
                join n in _context.News on mn.NewsId equals n.Id
                where idList.Contains(mn.MenuId) && !n.IsDeleted
                select new ValueTuple<int, int, string, bool>(mn.MenuId, n.Id, n.Title, n.IsPublished)
            ).ToListAsync(ct);
        }
    }
}