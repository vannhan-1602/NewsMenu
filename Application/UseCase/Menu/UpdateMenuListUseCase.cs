using Application.Common;
using Application.Request.Menu;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public UpdateMenuListUseCase(
            IMenuRepository menuRepository,
            INewsRepository newsRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _menuRepository = menuRepository;
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(UpdateMenuListRequest request, CancellationToken ct)
        {
            // Validate tất cả news ids trước khi mở transaction
            var allNewsIds = request.Items.SelectMany(item => item.NewsIds).Distinct();
            var existingNewsIds = allNewsIds.Any()
                ? await _newsRepository.GetExistingIdsAsync(allNewsIds, ct)
                : new List<int>();
            var existingNewsIdSet = new HashSet<int>(existingNewsIds);

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var requestedIds = request.Items.Select(item => item.Id).ToList();
                var menuList = new List<Menu>();
                await foreach (var menu in _menuRepository.Query()
                    .Where(m => requestedIds.Contains(m.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    menuList.Add(menu);
                }

                var menuLookup = menuList.ToDictionary(menu => menu.Id);

                // Batch load toàn bộ links hiện tại của tất cả menu
                var allCurrentLinks = await _menuRepository.GetMenuNewsByMenuIdsAsync(requestedIds, ct);
                var currentLinksLookup = allCurrentLinks
                    .GroupBy(link => link.MenuId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                int notFoundCount = 0;
                int totalInvalidNewsCount = 0;
                var linksToAdd = new List<MenuNews>();
                var linksToRemove = new List<MenuNews>();

                foreach (var item in request.Items)
                {
                    if (!menuLookup.TryGetValue(item.Id, out var menu))
                    {
                        notFoundCount++;
                        continue;
                    }

                    _mapper.Map(item, menu);

                    var validNewsIdsForItem = item.NewsIds.Distinct().Where(existingNewsIdSet.Contains).ToList();
                    totalInvalidNewsCount += item.NewsIds.Distinct().Count() - validNewsIdsForItem.Count;

                    // chỉ xóa link không còn dùng, chỉ thêm link mới
                    var currentLinks = currentLinksLookup.TryGetValue(menu.Id, out var links) ? links : new List<MenuNews>();
                    var currentNewsIdSet = currentLinks.Select(link => link.NewsId).ToHashSet();
                    var newNewsIdSet = new HashSet<int>(validNewsIdsForItem);

                    linksToRemove.AddRange(currentLinks.Where(link => !newNewsIdSet.Contains(link.NewsId)));
                    linksToAdd.AddRange(validNewsIdsForItem
                        .Where(newsId => !currentNewsIdSet.Contains(newsId))
                        .Select(newsId => new MenuNews { MenuId = menu.Id, NewsId = newsId }));
                }

                if (menuList.Count > 0)
                    _menuRepository.UpdateRange(menuList);
                if (linksToRemove.Count > 0)
                    _menuRepository.RemoveMenuNewsRange(linksToRemove);
                if (linksToAdd.Count > 0)
                    _menuRepository.AddMenuNewsRange(linksToAdd);

                await _unitOfWork.CommitAsync(ct);

                var messageParts = new List<string> { $"Cập nhật {menuList.Count} menu thành công." };
                if (notFoundCount > 0) messageParts.Add($"{notFoundCount} Id không tồn tại.");
                if (totalInvalidNewsCount > 0) messageParts.Add($"{totalInvalidNewsCount} NewsId không hợp lệ đã bị bỏ qua.");

                return new BaseResponse { Success = true, Message = string.Join(" ", messageParts) };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}