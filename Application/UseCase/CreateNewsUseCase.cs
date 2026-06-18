using Application.Common;
using Application.Request;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class CreateNewsUseCase : IRequestHandler<CreateNewsRequest, BaseResponse>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateNewsUseCase(
            INewsRepository newsRepository,
            IMenuRepository menuRepository,
            IUnitOfWork unitOfWork)
        {
            _newsRepository = newsRepository;
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(CreateNewsRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var news = new News
                {
                    Title = request.Title,
                    Content = request.Content,
                    Summary = request.Summary,
                    IsPublished = request.IsPublished
                };

                await _newsRepository.AddAsync(news, ct);

                
                var invalidIds = new List<int>();

                // Thêm check null cho an toàn trước khi gọi .Any()
                if (request.MenuIds != null && request.MenuIds.Any())
                {
                    // Kiểm tra Id nào thực sự tồn tại trong DB
                    var existingIds = await _menuRepository.GetExistingIdsAsync(request.MenuIds, ct);

                    
                    var existingSet = new HashSet<int>(existingIds);

                    // Thu thập Id không hợp lệ 
                    invalidIds = request.MenuIds
                        .Distinct()
                        .Where(id => !existingSet.Contains(id))
                        .ToList(); 

                    // Chỉ gán các Id hợp lệ
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
                    ? $"Tao news thanh cong. {invalidIds.Count} MenuId khong hop le da bi bo qua."
                    : "tao news thanh cong";

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