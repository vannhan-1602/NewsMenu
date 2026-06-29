using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<Menu?> GetBySlugAsync(string slug, CancellationToken ct = default);
        Task<List<MenuNews>> GetMenuNewsByMenuIdAsync(int menuId, CancellationToken ct = default);
        Task<List<MenuNews>> GetMenuNewsByMenuIdsAsync(IEnumerable<int> menuIds, CancellationToken ct = default);

        void AddMenuNewsRange(IEnumerable<MenuNews> menuNews);
        void RemoveMenuNewsRange(IEnumerable<MenuNews> menuNews);
    }
}
