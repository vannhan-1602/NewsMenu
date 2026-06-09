using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INewsReadRepository
    {
        Task<NewsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<NewsDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
        Task UpsertAsync(NewsDto news, CancellationToken ct = default);

        Task DeleteAsync(Guid id, CancellationToken ct = default); // xóa ở mongo   
    }
}
