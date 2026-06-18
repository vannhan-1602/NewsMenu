using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

        // Trả về IQueryable để Application layer tự Select 
        IQueryable<T> Query();

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        // Kiểm tra tồn tại theo danh sách id 
        Task<List<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);

        Task AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        // Update chỉ đánh dấu trạng thái Entry, KHÔNG tự SaveChanges 
        void Update(T entity);

        // Soft delete
        void SoftDelete(T entity);
    }
}
