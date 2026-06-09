using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<Menu?> GetBySlugAsync(string slug, CancellationToken ct = default);
    }
}
