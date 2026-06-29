using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NewsRepository : BaseRepository<News>, INewsRepository
    {
        public NewsRepository(AppDbContext context) : base(context) { }

        // Lấy toàn bộ liên kết MenuNews theo newsId
        public async Task<List<MenuNews>> GetMenuNewsByNewsIdAsync(int newsId, CancellationToken ct = default)
        {
            var result = new List<MenuNews>();
            await foreach (var menuNews in _context.MenuNews
                .Where(menuNews => menuNews.NewsId == newsId)
                .AsAsyncEnumerable()
                .WithCancellation(ct))
            {
                result.Add(menuNews);
            }
            return result;
        }

        // Lấy toàn bộ liên kết MenuNews theo nhiều newsId (batch)
        public async Task<List<MenuNews>> GetMenuNewsByNewsIdsAsync(IEnumerable<int> newsIds, CancellationToken ct = default)
        {
            var result = new List<MenuNews>();
            await foreach (var menuNews in _context.MenuNews
                .Where(menuNews => newsIds.Contains(menuNews.NewsId))
                .AsAsyncEnumerable()
                .WithCancellation(ct))
            {
                result.Add(menuNews);
            }
            return result;
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
