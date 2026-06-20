using Domain.Entities;

namespace Domain.Interfaces
{
    // Không cần thêm method riêng - GetByIdAsync kế thừa từ IRepository<Country> là đủ
    public interface ICountryRepository : IRepository<Country>
    {
    }
}
