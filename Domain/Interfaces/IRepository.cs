using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

        // Trả về IQueryable để Application layer tự Select - tối ưu, không tải dư cột
        IQueryable<T> Query();

        // Kiểm tra tồn tại theo danh sách id - dùng khi gán quan hệ N-N
        Task<List<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);

        Task AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        // Update dùng chung cho cả update thường lẫn soft-delete (set IsDeleted = true rồi gọi Update)
        // Chỉ đánh dấu trạng thái Entry, KHÔNG tự SaveChanges
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
    }
}
