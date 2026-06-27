using Application.Common;
using Application.Request.Menu;
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
            // Validate news ids trước khi mở transaction
            var invalidIds = new List<int>();
            var existingIds = new List<int>();
            if (request.NewsIds.Count > 0)
            {
                existingIds = await _newsRepository.GetExistingIdsAsync(request.NewsIds, ct);
                var existingSet = new HashSet<int>(existingIds);
                invalidIds = request.NewsIds.Distinct().Where(id => !existingSet.Contains(id)).ToList();
            }

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
                await _unitOfWork.SaveChangesAsync(ct);

                if (existingIds.Count > 0)
                {
                    var links = existingIds.Select(newsId => new MenuNews { MenuId = menu.Id, NewsId = newsId });
                    _menuRepository.AddMenuNewsRange(links);
                }

                await _unitOfWork.CommitAsync(ct);

                var message = invalidIds.Count > 0
                    ? $"Tạo menu thành công. {invalidIds.Count} NewsId không tồn tại đã bị bỏ qua."
                    : "Tạo menu thành công.";

                return new BaseResponse { Success = true, Message = message, Id = menu.Id, InvalidIds = invalidIds };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}
