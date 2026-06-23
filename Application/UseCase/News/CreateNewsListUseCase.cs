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
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
              
                var allMenuIds = request.Items.SelectMany(x => x.MenuIds).Distinct();
                var existingMenuIds = allMenuIds.Any()
                    ? await _menuRepository.GetExistingIdsAsync(allMenuIds, ct)
                    : new List<int>();
                var existingMenuSet = new HashSet<int>(existingMenuIds);

                var newsList = request.Items.Select(item => new News
                {
                    Title = item.Title,
                    Content = item.Content,
                    Summary = item.Summary,
                    IsPublished = item.IsPublished
                }).ToList();

                await _newsRepository.AddRangeAsync(newsList, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                var allLinks = new List<MenuNews>();
                var totalInvalid = 0;

                for (int i = 0; i < request.Items.Count; i++)
                {
                    var validIds = request.Items[i].MenuIds.Distinct().Where(existingMenuSet.Contains).ToList();
                    totalInvalid += request.Items[i].MenuIds.Distinct().Count() - validIds.Count;

                    allLinks.AddRange(validIds.Select(menuId => new MenuNews
                    {
                        MenuId = menuId,
                        NewsId = newsList[i].Id
                    }));
                }

                if (allLinks.Count > 0)
                    _newsRepository.AddMenuNewsRange(allLinks);

                await _unitOfWork.CommitAsync(ct);

                var message = totalInvalid > 0
                    ? $"Tao {newsList.Count} news thanh cong. {totalInvalid} MenuId khong hop le da bi bo qua."
                    : $"Tao {newsList.Count} news thanh cong";

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
