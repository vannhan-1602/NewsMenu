using Application.Common;
using Application.Request.News;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class CreateNewsListUseCase : IRequestHandler<CreateNewsListRequest, BaseResponse>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateNewsListUseCase(
            INewsRepository newsRepository,
            IMenuRepository menuRepository,
            IUnitOfWork unitOfWork)
        {
            _newsRepository = newsRepository;
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(CreateNewsListRequest request, CancellationToken ct)
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
                var newsList = request.Items.Select(item => new News
                {
                    Title = item.Title,
                    Content = item.Content,
                    Summary = item.Summary,
                    IsPublished = item.IsPublished
                }).ToList();

                await _newsRepository.AddRangeAsync(newsList, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                var allMenuNewsLinks = new List<MenuNews>();
                int totalInvalidMenuCount = 0;

                for (int index = 0; index < request.Items.Count; index++)
                {
                    var validMenuIds = request.Items[index].MenuIds.Distinct().Where(existingMenuIdSet.Contains).ToList();
                    totalInvalidMenuCount += request.Items[index].MenuIds.Distinct().Count() - validMenuIds.Count;

                    allMenuNewsLinks.AddRange(validMenuIds.Select(menuId => new MenuNews
                    {
                        MenuId = menuId,
                        NewsId = newsList[index].Id
                    }));
                }

                if (allMenuNewsLinks.Count > 0)
                    _newsRepository.AddMenuNewsRange(allMenuNewsLinks);

                await _unitOfWork.CommitAsync(ct);

                var message = totalInvalidMenuCount > 0
                    ? $"Tạo {newsList.Count} news thành công. {totalInvalidMenuCount} MenuId không hợp lệ đã bị bỏ qua."
                    : $"Tạo {newsList.Count} news thành công.";

                return new BaseResponse { Success = true, Message = message };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}