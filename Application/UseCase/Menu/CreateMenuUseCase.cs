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
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                // Tạo Menu
                var menu = new Menu
                {
                    Name = request.Name,
                    Slug = request.Slug,
                    DisplayOrder = request.DisplayOrder
                };
                
                await _menuRepository.AddAsync(menu, ct);

                // Lưu vào cơ sở dữ liệu để có được Id của Menu
                await _unitOfWork.SaveChangesAsync(ct);

                var invalidIds = new List<int>();

                if (request.NewsIds.Count > 0)
                {
                    // Lấy danh sách NewsId tồn tại trong db
                    var existingIds = await _newsRepository.GetExistingIdsAsync(request.NewsIds, ct);
                    var existingSet = new HashSet<int>(existingIds);

                    invalidIds = request.NewsIds
                        .Distinct()
                        .Where(id => !existingSet.Contains(id))
                        .ToList();

                    if (existingIds.Count > 0)
                    {
                        var links = existingIds.Select(newsId => new MenuNews
                        {
                            MenuId = menu.Id,
                            NewsId = newsId
                        });
                        _menuRepository.AddMenuNewsRange(links);
                    }
                }

                //insert MenuNews+ commit transaction
                await _unitOfWork.CommitAsync(ct);

                var message = invalidIds.Count > 0
                    ? $"Tao menu thanh cong. {invalidIds.Count} NewsId khong ton tai da bi bo qua."
                    : "Tao menu thanh cong";

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
