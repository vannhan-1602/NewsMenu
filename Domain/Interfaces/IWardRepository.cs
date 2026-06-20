using Domain.Entities;

namespace Domain.Interfaces
{
    // Không cần thêm method riêng - GetByIdAsync kế thừa từ IRepository<Ward> là đủ
    // (dùng khi Create/Update News cần validate WardId tồn tại)
    // Việc lấy WardInfo/FullAddress cho hiển thị nằm trong NewsRepository.Query()
    // qua navigation Ward.Parent.Country - 1 câu Select duy nhất, không đệ quy C#
    public interface IWardRepository : IRepository<Ward>
    {
    }
}
