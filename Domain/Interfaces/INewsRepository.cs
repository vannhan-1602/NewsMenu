using Domain.Entities;

namespace Domain.Interfaces
{
    public interface INewsRepository : IRepository<News>
    {

        Task<List<MenuNews>> GetMenuNewsByNewsIdAsync(int newsId, CancellationToken ct = default);
        Task<List<MenuNews>> GetMenuNewsByNewsIdsAsync(IEnumerable<int> newsIds, CancellationToken ct = default);

        void AddMenuNewsRange(IEnumerable<MenuNews> menuNews);
        void RemoveMenuNewsRange(IEnumerable<MenuNews> menuNews);
    }
}
