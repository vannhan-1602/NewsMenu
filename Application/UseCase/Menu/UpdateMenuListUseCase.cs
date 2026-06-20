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
                var ids = request.Items.Select(x => x.Id).ToList();

                // Lấy toàn bộ Menu cần update trong 1 query, không loop GetByIdAsync từng cái
                // AsAsyncEnumerable + await foreach thay cho ToListAsync()
                var menus = new List<Menu>();
                await foreach (var menu in _menuRepository.Query()
                    .Where(m => ids.Contains(m.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    menus.Add(menu);
                }
                var menuDict = menus.ToDictionary(m => m.Id);

                // Không ToList() - truyền thẳng IEnumerable, GetExistingIdsAsync chỉ enumerate đúng 1 lần
                var allNewsIds = request.Items.SelectMany(x => x.NewsIds).Distinct();
                var existingNewsIds = allNewsIds.Any()
                    ? await _newsRepository.GetExistingIdsAsync(allNewsIds, ct)
                    : new List<int>();
                var existingNewsSet = new HashSet<int>(existingNewsIds);

                int notFoundCount = 0, totalInvalid = 0;
                var allToAdd = new List<MenuNews>();
                var allToRemove = new List<MenuNews>();

                foreach (var item in request.Items)
                {
                    if (!menuDict.TryGetValue(item.Id, out var menu))
                    {
                        notFoundCount++;
                        continue;
                    }

                    menu.Name = item.Name;
                    menu.Slug = item.Slug;
                    menu.DisplayOrder = item.DisplayOrder;

                    var validIds = item.NewsIds.Distinct().Where(existingNewsSet.Contains).ToList();
                    totalInvalid += item.NewsIds.Distinct().Count() - validIds.Count;

                    // Đơn giản hóa: xóa hết MenuNews hiện có của menu này rồi thêm lại từ đầu
                    var currentLinks = await _menuRepository.GetMenuNewsByMenuIdAsync(menu.Id, ct);
                    allToRemove.AddRange(currentLinks);
                    allToAdd.AddRange(validIds.Select(newsId => new MenuNews { MenuId = menu.Id, NewsId = newsId }));
                }

                if (menus.Count > 0)
                    _menuRepository.UpdateRange(menus);
                if (allToRemove.Count > 0)
                    _menuRepository.RemoveMenuNewsRange(allToRemove);
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
