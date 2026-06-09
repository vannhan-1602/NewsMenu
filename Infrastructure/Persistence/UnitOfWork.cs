using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool _disposed;

        private IMenuRepository? _menus;
        private INewsRepository? _news;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IMenuRepository Menus
            => _menus ??= new MenuRepository(_context);

        public INewsRepository News
            => _news ??= new NewsRepository(_context);

        public async Task<int> CommitAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
        public async Task RollbackAsync()
        {
            foreach (var entry in _context.ChangeTracker.Entries())
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
