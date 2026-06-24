using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

       
        IQueryable<T> Query();

        Task<List<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);

        Task AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
    }
}
