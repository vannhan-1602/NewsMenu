using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMenuRepository Menus { get; }
        INewsRepository News { get; }
        Task<int> CommitAsync(CancellationToken ct = default);
        Task RollbackAsync();
    }
}
