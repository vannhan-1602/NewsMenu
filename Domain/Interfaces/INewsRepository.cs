using Domain.Entities;

namespace Domain.Interfaces
{
    public interface INewsRepository : IRepository<News>
    {
        // Lấy MenuNews hiện có của 1 news - dùng khi Update để biết phải thêm/xóa link nào
        Task<List<MenuNews>> GetMenuNewsByNewsIdAsync(int newsId, CancellationToken ct = default);

        void AddMenuNewsRange(IEnumerable<MenuNews> menuNews);
        void RemoveMenuNewsRange(IEnumerable<MenuNews> menuNews);
    }
}
