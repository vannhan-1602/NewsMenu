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
        //lấy theo id - chỉ lấy chưa xóa mềm
        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
          
            return await _dbSet
                .Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);
        }
        // trả về danh sách các bản ghi chưa xóa mềm , dùng AsNoTracking để tăng hiệu suất khi chỉ đọc dữ liệu
        public IQueryable<T> Query()
        {
          
            return _dbSet.Where(x => !x.IsDeleted).AsNoTracking();
        }
        // trả về danh sách các id tồn tại trong cơ sở dữ liệu, chỉ lấy các bản ghi chưa xóa mềm
        public async Task<List<int>> GetExistingIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
        {
          
            var idList = ids.Distinct().ToList();
            var result = new List<int>();
            await foreach (var id in _dbSet
                .Where(x => !x.IsDeleted && idList.Contains(x.Id))
                .Select(x => x.Id)          
                .AsAsyncEnumerable()
                .WithCancellation(ct))
            {
                result.Add(id);
            }
            return result;
        }
        // thêm mới một bản ghi, tự động set CreatedAt và UpdatedAt
        public async Task AddAsync(T entity, CancellationToken ct = default)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity, ct);
        }
        // thêm mới nhiều bản ghi, tự động set CreatedAt và UpdatedAt
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
        // cập nhật một bản ghi, tự động set UpdatedAt
        public void Update(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
          
        }
        // cập nhật nhiều bản ghi, tự động set UpdatedAt
        public void UpdateRange(IEnumerable<T> entities)
        {
            var now = DateTime.UtcNow;
            foreach (var entity in entities)
                entity.UpdatedAt = now;
            _dbSet.UpdateRange(entities);
        }
        //lay danh sach da xoa
        public IQueryable<T> QueryDeleted()
        {
            return _dbSet.Where(x => x.IsDeleted).AsNoTracking();
        }

        public void Restore(T entity)
        {
            entity.IsDeleted = false;
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
        public async Task<T?> GetByIdIncludeDeletedAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet.Where(x => x.Id == id).FirstOrDefaultAsync(ct);
        }
    }
}
