using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface INewsRepository : IRepository<News>
    {
        Task<News?> GetWithMenusAsync(Guid newsId, CancellationToken ct = default);
    }
}
