using Application.Common;
using Application.Request.News;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class CreateNewsUseCase : IRequestHandler<CreateNewsRequest, BaseResponse>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateNewsUseCase(
            INewsRepository newsRepository,
            IMenuRepository menuRepository,
            IWardRepository wardRepository,
            IUnitOfWork unitOfWork)
        {
            _newsRepository = newsRepository;
            _menuRepository = menuRepository;
            _wardRepository = wardRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(CreateNewsRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                // Kiểm tra WardId nếu có
                int? wardId = null;
                bool wardInvalid = false;
                if (request.WardId.HasValue)
                {
                    var ward = await _wardRepository.GetByIdAsync(request.WardId.Value, ct);
                    if (ward != null)
                    {
                        wardId = ward.Id;
                    }
                    else
                    {
                        wardInvalid = true;
                    }
                }
                // Tạo News mới
                var news = new News
                {
                    Title = request.Title,
                    Content = request.Content,
                    Summary = request.Summary,
                    IsPublished = request.IsPublished,
                    WardId = wardId,
                    Address = request.Address
                };

                await _newsRepository.AddAsync(news, ct);

                // Lưu News trước để lấy Id thật trước khi gán vào MenuNews
                await _unitOfWork.SaveChangesAsync(ct);

                var invalidIds = new List<int>();
                
                if (request.MenuIds.Count > 0)
                {
                    var existingIds = await _menuRepository.GetExistingIdsAsync(request.MenuIds, ct);
                    var existingSet = new HashSet<int>(existingIds);

                    invalidIds = request.MenuIds
                        .Distinct()
                        .Where(id => !existingSet.Contains(id))
                        .ToList();

                    if (existingIds.Count > 0)
                    {
                        var links = existingIds.Select(menuId => new MenuNews
                        {
                            MenuId = menuId,
                            NewsId = news.Id
                        });
                        _newsRepository.AddMenuNewsRange(links);
                    }
                }

                await _unitOfWork.CommitAsync(ct);

                var parts = new List<string>();
                parts.Add(invalidIds.Count > 0
                    ? $"Tao news thanh cong. {invalidIds.Count} MenuId khong hop le da bi bo qua."
                    : "Tao news thanh cong");
                if (wardInvalid)
                {
                    parts.Add("WardId khong ton tai, da bo qua.");
                }

                return new BaseResponse
                {
                    Success = true,
                    Message = string.Join(" ", parts),
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
