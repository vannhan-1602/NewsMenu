using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NewsRepository : BaseRepository<News>, INewsRepository
    {
        public NewsRepository(AppDbContext context) : base(context) { }

        public async Task<List<MenuNews>> GetMenuNewsByNewsIdAsync(int newsId, CancellationToken ct = default)
        {
           
            var result = new List<MenuNews>();
            await foreach (var mn in _context.MenuNews
                .Where(x => x.NewsId == newsId)
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
