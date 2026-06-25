using Application.Common;
using Application.Request.Menu;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class UpdateMenuListUseCase : IRequestHandler<UpdateMenuListRequest, BaseResponse>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMenuListUseCase(
            IMenuRepository menuRepository,
            INewsRepository newsRepository,
            IUnitOfWork unitOfWork)
        {
            _menuRepository = menuRepository;
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(UpdateMenuListRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                // Lấy tất cả menu theo danh sách Id từ request
                var ids = request.Items.Select(x => x.Id).ToList();
                var menus = new List<Menu>();

                // Sử dụng AsAsyncEnumerable để xử lý dữ liệu theo từng phần
                // tránh tải toàn bộ vào bộ nhớ
                await foreach (var menu in _menuRepository.Query()
                    .Where(m => ids.Contains(m.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    menus.Add(menu);
                }

                // Tạo dictionary để tra cứu nhanh menu theo Id
                var menuDict = menus.ToDictionary(m => m.Id);

                // Lấy tất cả NewsIds từ request và bỏ trùng lặp
                var allNewsIds = request.Items.SelectMany(x => x.NewsIds).Distinct();

                // Lấy danh sách NewsId tồn tại trong db
                var existingNewsIds = allNewsIds.Any()
                    ? await _newsRepository.GetExistingIdsAsync(allNewsIds, ct)
                    : new List<int>();
                var existingNewsSet = new HashSet<int>(existingNewsIds);

                int notFoundCount = 0, totalInvalid = 0;

                // Danh sách các liên kết MenuNews cần thêm và xóa
                var allToAdd = new List<MenuNews>();
                var allToRemove = new List<MenuNews>();

                // Cập nhật từng menu theo request
                foreach (var item in request.Items)
                {
                    if (!menuDict.TryGetValue(item.Id, out var menu))
                    {
                        notFoundCount++;
                        continue;
                    }
                    // Cập nhật thông tin menu
                    menu.Name = item.Name;
                    menu.Slug = item.Slug;
                    menu.DisplayOrder = item.DisplayOrder;
                    // Lọc các NewsId hợp lệ
                    var validIds = item.NewsIds.Distinct().Where(existingNewsSet.Contains).ToList();

                    // Tính số lượng NewsId không hợp lệ
                    totalInvalid += item.NewsIds.Distinct().Count() - validIds.Count;

                    var currentLinks = await _menuRepository.GetMenuNewsByMenuIdAsync(menu.Id, ct);
                    allToRemove.AddRange(currentLinks);
                    allToAdd.AddRange(validIds.Select(newsId => new MenuNews { MenuId = menu.Id, NewsId = newsId }));
                }
                //cập nhập all menu
                if (menus.Count > 0)
                    _menuRepository.UpdateRange(menus);
                //xóa các liên kết cũ
                if (allToRemove.Count > 0)
                    _menuRepository.RemoveMenuNewsRange(allToRemove);
                //thêm các liên kết mới
                if (allToAdd.Count > 0)
                    _menuRepository.AddMenuNewsRange(allToAdd);

                await _unitOfWork.CommitAsync(ct);

                var parts = new List<string> { $"Cap nhat {menus.Count} menu thanh cong" };
                if (notFoundCount > 0) parts.Add($"{notFoundCount} Id khong ton tai");
                if (totalInvalid > 0) parts.Add($"{totalInvalid} NewsId khong hop le da bi bo qua");

                return new BaseResponse { Success = true, Message = string.Join(". ", parts) };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}
