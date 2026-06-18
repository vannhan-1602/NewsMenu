using Application.Common;
using Application.Request;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class UpdateMenuUseCase : IRequestHandler<UpdateMenuRequest, BaseResponse>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMenuUseCase(
            IMenuRepository menuRepository,
            INewsRepository newsRepository,
            IUnitOfWork unitOfWork)
        {
            _menuRepository = menuRepository;
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(UpdateMenuRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                
                var menu = await _menuRepository.GetByIdAsync(request.Id, ct);
                if (menu == null)
                    return new BaseResponse { Success = false, Message = "menu khong ton tai hoac da bi xoa" };

                menu.Name = request.Name;
                menu.Slug = request.Slug;
                menu.DisplayOrder = request.DisplayOrder;

                _menuRepository.Update(menu);

               
                await _menuRepository.RemoveMenuNewsByMenuIdAsync(menu.Id, ct);

                
                var invalidIds = new List<int>();

                
                if (request.NewsIds != null && request.NewsIds.Any())
                {
                    var existingIds = await _newsRepository.GetExistingIdsAsync(request.NewsIds, ct);

                    
                    var existingSet = new HashSet<int>(existingIds);

                    invalidIds = request.NewsIds
                        .Distinct()
                        .Where(id => !existingSet.Contains(id))
                        .ToList();

                    if (existingIds.Any())
                    {
                        var links = existingIds.Select(newsId => new MenuNews
                        {
                            MenuId = menu.Id,
                            NewsId = newsId
                        });
                        await _menuRepository.AddMenuNewsRangeAsync(links, ct);
                    }
                }

                await _unitOfWork.CommitAsync(ct);

                var message = invalidIds.Any()
                    ? $"Cap nhap menu thanh cong. {invalidIds.Count} NewsId khong hop le da bi bo qua."
                    : "cap nhap menu thanh cong";

                return new BaseResponse
                {
                    Success = true,
                    Message = message,
                    Id = menu.Id,
                    InvalidIds = invalidIds
                };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}