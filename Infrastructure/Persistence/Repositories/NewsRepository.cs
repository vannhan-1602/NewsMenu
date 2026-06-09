using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class NewsRepository : BaseRepository<News>, INewsRepository
    {
        public NewsRepository(AppDbContext context) : base(context) { }

        public async Task<News?> GetWithMenusAsync(Guid newsId, CancellationToken ct = default)
            => await _dbSet
                .Include(n => n.MenuNews)
                    .ThenInclude(mn => mn.Menu)
                .FirstOrDefaultAsync(n => n.Id == newsId, ct);
    }
}
