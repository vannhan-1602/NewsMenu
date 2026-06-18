using Application.Common;
using Application.Request;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class CreateMenuUseCase : IRequestHandler<CreateMenuRequest, BaseResponse>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateMenuUseCase(
            IMenuRepository menuRepository,
            INewsRepository newsRepository,
            IUnitOfWork unitOfWork)
        {
            _menuRepository = menuRepository;
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(CreateMenuRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var menu = new Menu
                {
                    Name = request.Name,
                    Slug = request.Slug,
                    DisplayOrder = request.DisplayOrder
                };

                await _menuRepository.AddAsync(menu, ct);

                // Lưu Menu trước để lấy Id thật (identity) trước khi gán vào MenuNews
                await _unitOfWork.SaveChangesAsync(ct);


                var invalidIds = new List<int>();

                if (request.NewsIds != null && request.NewsIds.Any())
                {
                    // Kiểm tra Id nào thực sự tồn tại trong DB
                    var existingIds = await _newsRepository.GetExistingIdsAsync(request.NewsIds, ct);


                    var existingSet = new HashSet<int>(existingIds);

                    // Thu thập Id không hợp lệ
                    invalidIds = request.NewsIds
                        .Distinct()
                        .Where(id => !existingSet.Contains(id))
                        .ToList();

                    // Chỉ gán các Id hợp lệ
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
                    ? $"Tao menu thanh cong {invalidIds.Count} NewsId khong ton tai da bi bo qua."
                    : "tao menu thanh cong";

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