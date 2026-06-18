using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces
{

    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync(CancellationToken ct = default);

        
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task<int> CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }
}
