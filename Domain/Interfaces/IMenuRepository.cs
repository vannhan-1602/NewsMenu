using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<Menu?> GetBySlugAsync(string slug, CancellationToken ct = default);

        // Lấy MenuNews hiện có của 1 menu - dùng khi Update để biết phải thêm/xóa link nào
        Task<List<MenuNews>> GetMenuNewsByMenuIdAsync(int menuId, CancellationToken ct = default);

        void AddMenuNewsRange(IEnumerable<MenuNews> menuNews);
        void RemoveMenuNewsRange(IEnumerable<MenuNews> menuNews);
    }
}
