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

        public async Task<List<MenuNews>> GetMenuNewsByMenuIdAsync(int menuId, CancellationToken ct = default)
        {
            // AsAsyncEnumerable + await foreach thay cho ToListAsync()
            var result = new List<MenuNews>();
            await foreach (var mn in _context.MenuNews
                .Where(x => x.MenuId == menuId)
                .AsAsyncEnumerable()
                .WithCancellation(ct))
            {
                result.Add(mn);
            }
            return result;
        }

        public void AddMenuNewsRange(IEnumerable<MenuNews> menuNews)
        {
            var now = DateTime.UtcNow;
            foreach (var mn in menuNews)
                mn.AssignedAt = now;
            _context.MenuNews.AddRange(menuNews);
        }

        public void RemoveMenuNewsRange(IEnumerable<MenuNews> menuNews)
        {
            _context.MenuNews.RemoveRange(menuNews);
        }
    }
}
