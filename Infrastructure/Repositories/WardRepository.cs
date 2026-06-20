using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class WardRepository : BaseRepository<Ward>, IWardRepository
    {
        public WardRepository(AppDbContext context) : base(context) { }
    }
}
