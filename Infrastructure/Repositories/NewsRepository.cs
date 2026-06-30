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
            return GetMenuNewsAsync(menuNews => menuNews.NewsId == newsId, ct);
        }

        // Lấy toàn bộ liên kết MenuNews theo nhiều newsId (batch)
        public Task<List<MenuNews>> GetMenuNewsByNewsIdsAsync(IEnumerable<int> newsIds, CancellationToken ct = default)
        {
            return GetMenuNewsAsync(menuNews => newsIds.Contains(menuNews.NewsId), ct);
        }
    }
}