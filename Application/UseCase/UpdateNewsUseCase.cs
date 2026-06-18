using Application.Common;
using Application.Request;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class UpdateNewsUseCase : IRequestHandler<UpdateNewsRequest, BaseResponse>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateNewsUseCase(
            INewsRepository newsRepository,
            IMenuRepository menuRepository,
            IUnitOfWork unitOfWork)
        {
            _newsRepository = newsRepository;
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(UpdateNewsRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var news = await _newsRepository.GetByIdAsync(request.Id, ct);
                if (news == null)
                    return new BaseResponse { Success = false, Message = "News khong ton tai hoac da bi xoa" };

                news.Title = request.Title;
                news.Content = request.Content;
                news.Summary = request.Summary;
                news.IsPublished = request.IsPublished;

                _newsRepository.Update(news);

              
                await _newsRepository.RemoveMenuNewsByNewsIdAsync(news.Id, ct);

                
                var invalidIds = new List<int>();

               
                if (request.MenuIds != null && request.MenuIds.Any())
                {
                    var existingIds = await _menuRepository.GetExistingIdsAsync(request.MenuIds, ct);

                  
                    var existingSet = new HashSet<int>(existingIds);

                    invalidIds = request.MenuIds
                        .Distinct()
                        .Where(id => !existingSet.Contains(id))
                        .ToList();

                    if (existingIds.Any())
                    {
                        var links = existingIds.Select(menuId => new MenuNews
                        {
                            MenuId = menuId,
                            NewsId = news.Id
                        });
                        await _newsRepository.AddMenuNewsRangeAsync(links, ct);
                    }
                }

                await _unitOfWork.CommitAsync(ct);

                var message = invalidIds.Any()
                    ? $"cap nhap news thanh cong. {invalidIds.Count} MenuId khong hop le da bi bo qua."
                    : "cap nhap news thanh cong";

                return new BaseResponse
                {
                    Success = true,
                    Message = message,
                   
                    Id = news.Id,
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