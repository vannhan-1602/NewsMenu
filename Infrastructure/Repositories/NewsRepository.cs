using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NewsRepository : BaseRepository<News>, INewsRepository
    {
        public NewsRepository(AppDbContext context) : base(context) { }

        public async Task AddMenuNewsRangeAsync(IEnumerable<MenuNews> menuNews, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            foreach (var mn in menuNews)
                mn.AssignedAt = now;

            await _context.MenuNews.AddRangeAsync(menuNews, ct);
        }

        public async Task RemoveMenuNewsByNewsIdAsync(int newsId, CancellationToken ct = default)
        {
            var existing = await _context.MenuNews
                .Where(x => x.NewsId == newsId)
                .ToListAsync(ct);

            _context.MenuNews.RemoveRange(existing);
        }

       
        public async Task<List<(int MenuId, string Name, string Slug, int DisplayOrder)>> GetMenusOfNewsAsync(int newsId, CancellationToken ct = default)
        {
            return await (
                from mn in _context.MenuNews
                join m in _context.Menus on mn.MenuId equals m.Id
                where mn.NewsId == newsId && !m.IsDeleted
                select new ValueTuple<int, string, string, int>(m.Id, m.Name, m.Slug, m.DisplayOrder)
            ).ToListAsync(ct);
        }

        // 1 query cho cả page - tránh N+1
        public async Task<List<(int NewsId, int MenuId, string Name, string Slug, int DisplayOrder)>> GetMenusOfNewsListAsync(IEnumerable<int> newsIds, CancellationToken ct = default)
        {
            var idList = newsIds.Distinct().ToList();
            return await (
                from mn in _context.MenuNews
                join m in _context.Menus on mn.MenuId equals m.Id
                where idList.Contains(mn.NewsId) && !m.IsDeleted
                select new ValueTuple<int, int, string, string, int>(mn.NewsId, m.Id, m.Name, m.Slug, m.DisplayOrder)
            ).ToListAsync(ct);
        }
    }
}