using Application.Common;
using Application.Request.News;
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

        public UpdateNewsListUseCase(
            INewsRepository newsRepository,
            IMenuRepository menuRepository,
            IUnitOfWork unitOfWork)
        {
            _newsRepository = newsRepository;
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(UpdateNewsListRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var ids = request.Items.Select(x => x.Id).ToList();

               
                var newsList = new List<News>();
                await foreach (var news in _newsRepository.Query()
                    .Where(n => ids.Contains(n.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    newsList.Add(news);
                }
                var newsDict = newsList.ToDictionary(n => n.Id);

             
                var allMenuIds = request.Items.SelectMany(x => x.MenuIds).Distinct();
                var existingMenuIds = allMenuIds.Any()
                    ? await _menuRepository.GetExistingIdsAsync(allMenuIds, ct)
                    : new List<int>();
                var existingMenuSet = new HashSet<int>(existingMenuIds);

                int notFoundCount = 0, totalInvalid = 0;
                var allToAdd = new List<MenuNews>();
                var allToRemove = new List<MenuNews>();

                foreach (var item in request.Items)
                {
                    if (!newsDict.TryGetValue(item.Id, out var news))
                    {
                        notFoundCount++;
                        continue;
                    }

                    news.Title = item.Title;
                    news.Content = item.Content;
                    news.Summary = item.Summary;
                    news.IsPublished = item.IsPublished;

                    var validIds = item.MenuIds.Distinct().Where(existingMenuSet.Contains).ToList();
                    totalInvalid += item.MenuIds.Distinct().Count() - validIds.Count;

                  
                    var currentLinks = await _newsRepository.GetMenuNewsByNewsIdAsync(news.Id, ct);
                    allToRemove.AddRange(currentLinks);
                    allToAdd.AddRange(validIds.Select(menuId => new MenuNews { MenuId = menuId, NewsId = news.Id }));
                }

                if (newsList.Count > 0)
                    _newsRepository.UpdateRange(newsList);
                if (allToRemove.Count > 0)
                    _newsRepository.RemoveMenuNewsRange(allToRemove);
                if (allToAdd.Count > 0)
                    _newsRepository.AddMenuNewsRange(allToAdd);

                await _unitOfWork.CommitAsync(ct);

                var parts = new List<string> { $"Cap nhat {newsList.Count} news thanh cong" };
                if (notFoundCount > 0) parts.Add($"{notFoundCount} Id khong ton tai");
                if (totalInvalid > 0) parts.Add($"{totalInvalid} MenuId khong hop le da bi bo qua");

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
