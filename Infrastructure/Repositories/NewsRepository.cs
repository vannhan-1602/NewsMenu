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
        public Task<List<MenuNews>> GetMenuNewsByNewsIdAsync(int newsId, CancellationToken ct = default)
        {
            return _context.MenuNews.Where(mn => mn.NewsId == newsId).ToListAsync(ct);
        }

        // Lấy toàn bộ liên kết MenuNews theo nhiều newsId (batch)
        public Task<List<MenuNews>> GetMenuNewsByNewsIdsAsync(IEnumerable<int> newsIds, CancellationToken ct = default)
        {
            return _context.MenuNews.Where(mn => newsIds.Contains(mn.NewsId)).ToListAsync(ct);
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