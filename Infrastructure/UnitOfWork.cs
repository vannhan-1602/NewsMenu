using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task<int> CommitAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _context.SaveChangesAsync(ct);

                if (_transaction != null)
                {
                    await _transaction.CommitAsync(ct);
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }

                return result;
            }
            catch
            {
                await RollbackAsync(ct);
                throw;
            }
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
