
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
namespace Infrastructure.Persistence.Repositories
{
    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        public MenuRepository(AppDbContext context) : base(context) { }

        public async Task<Menu?> GetBySlugAsync(string slug, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(m => m.Slug == slug, ct);
    }
}
