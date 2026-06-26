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
            // Validate ward trước khi mở transaction
            int? resolvedWardId = null;
            bool wardNotFound = false;
            if (request.WardId.HasValue)
            {
                var ward = await _wardRepository.GetByIdAsync(request.WardId.Value, ct);
                if (ward != null)
                    resolvedWardId = ward.Id;
                else
                    wardNotFound = true;
            }

            // Validate menu ids trước khi mở transaction
            var validMenuIds = new List<int>();
            var invalidMenuIds = new List<int>();
            if (request.MenuIds.Count > 0)
            {
                var existingMenuIds = await _menuRepository.GetExistingIdsAsync(request.MenuIds, ct);
                var existingMenuSet = new HashSet<int>(existingMenuIds);
                validMenuIds = existingMenuIds;
                invalidMenuIds = request.MenuIds.Distinct().Where(id => !existingMenuSet.Contains(id)).ToList();
            }

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var news = new News
                {
                    Title = request.Title,
                    Content = request.Content,
                    Summary = request.Summary,
                    IsPublished = request.IsPublished,
                    WardId = resolvedWardId,
                    Address = request.Address
                };

                await _newsRepository.AddAsync(news, ct);

                // Lưu trước để có Id thật rồi mới gán vào MenuNews
                await _unitOfWork.SaveChangesAsync(ct);

                if (validMenuIds.Count > 0)
                {
                    var menuNewsLinks = validMenuIds.Select(menuId => new MenuNews
                    {
                        MenuId = menuId,
                        NewsId = news.Id
                    });
                    _newsRepository.AddMenuNewsRange(menuNewsLinks);
                }

                await _unitOfWork.CommitAsync(ct);

                var messageParts = new List<string>();
                messageParts.Add(invalidMenuIds.Count > 0
                    ? $"Tạo news thành công. {invalidMenuIds.Count} MenuId không hợp lệ đã bị bỏ qua."
                    : "Tạo news thành công.");
                if (wardNotFound)
                    messageParts.Add("WardId không tồn tại, đã bỏ qua.");

                return new BaseResponse
                {
                    Success = true,
                    Message = string.Join(" ", messageParts),
                    Id = news.Id,
                    InvalidIds = invalidMenuIds
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