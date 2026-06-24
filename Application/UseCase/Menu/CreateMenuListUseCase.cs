using Application.Common;
using Application.Request.Menu;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class CreateMenuListUseCase : IRequestHandler<CreateMenuListRequest, BaseResponse>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateMenuListUseCase(
            IMenuRepository menuRepository,
            INewsRepository newsRepository,
            IUnitOfWork unitOfWork)
        {
            _menuRepository = menuRepository;
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(CreateMenuListRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                
                var allNewsIds = request.Items.SelectMany(x => x.NewsIds).Distinct();
                var existingNewsIds = allNewsIds.Any()
                    ? await _newsRepository.GetExistingIdsAsync(allNewsIds, ct)
                    : new List<int>();
                var existingNewsSet = new HashSet<int>(existingNewsIds);

                var menus = request.Items.Select(item => new Menu
                {
                    Name = item.Name,
                    Slug = item.Slug,
                    DisplayOrder = item.DisplayOrder
                }).ToList();

                await _menuRepository.AddRangeAsync(menus, ct);

                
                await _unitOfWork.SaveChangesAsync(ct);

                var allLinks = new List<MenuNews>();
                var totalInvalid = 0;

                for (int i = 0; i < request.Items.Count; i++)
                {
                    var validIds = request.Items[i].NewsIds.Distinct().Where(existingNewsSet.Contains).ToList();
                    totalInvalid += request.Items[i].NewsIds.Distinct().Count() - validIds.Count;

                    allLinks.AddRange(validIds.Select(newsId => new MenuNews
                    {
                        MenuId = menus[i].Id,
                        NewsId = newsId
                    }));
                }

                if (allLinks.Count > 0)
                    _menuRepository.AddMenuNewsRange(allLinks);

                await _unitOfWork.CommitAsync(ct);

                var message = totalInvalid > 0
                    ? $"Tao {menus.Count} menu thanh cong. {totalInvalid} NewsId khong hop le da bi bo qua."
                    : $"Tao {menus.Count} menu thanh cong";

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
