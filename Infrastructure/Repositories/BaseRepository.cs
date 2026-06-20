using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            // FirstOrDefaultAsync - chỉ lấy 1 row, dừng ngay khi gặp, không liên quan ToListAsync
            return await _dbSet
                .Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);
        }

        public IQueryable<T> Query()
        {
            // AsNoTracking: không track entity này trong Change Tracker - dùng cho query thuần đọc
            // Trả IQueryable, UseCase tự Select rồi AsAsyncEnumerable, không dùng ToListAsync
            return _dbSet.Where(x => !x.IsDeleted).AsNoTracking();
        }

        public async Task<List<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
        {
            // List ở đây là bắt buộc: List.Contains() được EF Core dịch ổn định thành SQL "IN (...)"
            // HashSet.Contains() từng không dịch được ở một số version EF Core (lỗi runtime "could not be translated")
            // nên giữ List cho phần truyền vào Where() của query DB - đây là chỗ không còn lựa chọn an toàn hơn
            var idList = ids.Distinct().ToList();

            // AsAsyncEnumerable + await foreach thay cho ToListAsync()
            // Đọc và gom từng id một, không buffer toàn bộ kết quả query cùng lúc
            var result = new List<int>();
            await foreach (var id in _dbSet
                .Where(x => !x.IsDeleted && idList.Contains(x.Id))
                .Select(x => x.Id)            // Select chỉ lấy cột Id, không tải nguyên Entity
                .AsAsyncEnumerable()
                .WithCancellation(ct))
            {
                result.Add(id);
            }
            return result;
        }

        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity, ct);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            foreach (var entity in entities)
            {
                entity.CreatedAt = now;
                entity.UpdatedAt = now;
            }
            await _dbSet.AddRangeAsync(entities, ct);
        }

        public void Update(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            // Soft-delete cũng đi qua hàm này: UseCase tự set entity.IsDeleted = true rồi gọi Update()
            // Không cần hàm SoftDelete() riêng vì bản chất chỉ là update 1 cột trạng thái
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            var now = DateTime.UtcNow;
            foreach (var entity in entities)
                entity.UpdatedAt = now;
            _dbSet.UpdateRange(entities);
        }
    }
}
