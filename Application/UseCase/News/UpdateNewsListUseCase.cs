using Application.Common;
using Application.Request.News;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class UpdateNewsListUseCase : IRequestHandler<UpdateNewsListRequest, BaseResponse>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateNewsListUseCase(
            INewsRepository newsRepository,
            IMenuRepository menuRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _newsRepository = newsRepository;
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(UpdateNewsListRequest request, CancellationToken ct)
        {
            // Validate tất cả menu ids trước khi mở transaction
            var allMenuIds = request.Items.SelectMany(item => item.MenuIds).Distinct();
            var existingMenuIds = allMenuIds.Any()
                ? await _menuRepository.GetExistingIdsAsync(allMenuIds, ct)
                : new List<int>();
            var existingMenuIdSet = new HashSet<int>(existingMenuIds);

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var requestedIds = request.Items.Select(item => item.Id).ToList();
                var newsList = new List<News>();
                await foreach (var news in _newsRepository.Query()
                    .Where(n => requestedIds.Contains(n.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    newsList.Add(news);
                }

                var newsLookup = newsList.ToDictionary(news => news.Id);

                int notFoundCount = 0;
                int totalInvalidMenuCount = 0;
                var linksToAdd = new List<MenuNews>();
                var linksToRemove = new List<MenuNews>();

                foreach (var item in request.Items)
                {
                    if (!newsLookup.TryGetValue(item.Id, out var news))
                    {
                        notFoundCount++;
                        continue;
                    }

                    _mapper.Map(item, news);

                    var validMenuIdsForItem = item.MenuIds.Distinct().Where(existingMenuIdSet.Contains).ToList();
                    totalInvalidMenuCount += item.MenuIds.Distinct().Count() - validMenuIdsForItem.Count;

                    // Diff-based: chỉ xóa link không dùng, chỉ thêm link mới
                    var currentLinks = await _newsRepository.GetMenuNewsByNewsIdAsync(news.Id, ct);
                    var currentMenuIdSet = currentLinks.Select(link => link.MenuId).ToHashSet();
                    var newMenuIdSet = new HashSet<int>(validMenuIdsForItem);

                    linksToRemove.AddRange(currentLinks.Where(link => !newMenuIdSet.Contains(link.MenuId)));
                    linksToAdd.AddRange(validMenuIdsForItem
                        .Where(menuId => !currentMenuIdSet.Contains(menuId))
                        .Select(menuId => new MenuNews { MenuId = menuId, NewsId = news.Id }));
                }

                if (newsList.Count > 0)
                    _newsRepository.UpdateRange(newsList);
                if (linksToRemove.Count > 0)
                    _newsRepository.RemoveMenuNewsRange(linksToRemove);
                if (linksToAdd.Count > 0)
                    _newsRepository.AddMenuNewsRange(linksToAdd);

                await _unitOfWork.CommitAsync(ct);

                var messageParts = new List<string> { $"Cập nhật {newsList.Count} news thành công." };
                if (notFoundCount > 0) messageParts.Add($"{notFoundCount} Id không tồn tại.");
                if (totalInvalidMenuCount > 0) messageParts.Add($"{totalInvalidMenuCount} MenuId không hợp lệ đã bị bỏ qua.");

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