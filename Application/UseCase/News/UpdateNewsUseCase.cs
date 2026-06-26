using Application.Common;
using Application.Request.News;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class UpdateNewsUseCase : IRequestHandler<UpdateNewsRequest, BaseResponse>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateNewsUseCase(
            INewsRepository newsRepository,
            IMenuRepository menuRepository,
            IWardRepository wardRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _newsRepository = newsRepository;
            _menuRepository = menuRepository;
            _wardRepository = wardRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(UpdateNewsRequest request, CancellationToken ct)
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
            // WardId == null => client muốn xóa địa chỉ, resolvedWardId giữ null

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
                var news = await _newsRepository.GetByIdAsync(request.Id, ct);
                if (news == null)
                    return new BaseResponse { Success = false, Message = "News không tồn tại hoặc đã bị xóa." };

                // AutoMapper map các field từ request sang entity
                _mapper.Map(request, news);
                // Gán WardId đã validate (null nếu client truyền null)
                news.WardId = resolvedWardId;

                _newsRepository.Update(news);

                // chỉ xóa link không còn dùng, chỉ thêm link mới
                var currentLinks = await _newsRepository.GetMenuNewsByNewsIdAsync(news.Id, ct);
                var currentMenuIdSet = currentLinks.Select(link => link.MenuId).ToHashSet();
                var newMenuIdSet = new HashSet<int>(validMenuIds);

                var linksToRemove = currentLinks.Where(link => !newMenuIdSet.Contains(link.MenuId)).ToList();
                var menuIdsToAdd = validMenuIds.Where(menuId => !currentMenuIdSet.Contains(menuId)).ToList();

                if (linksToRemove.Count > 0)
                    _newsRepository.RemoveMenuNewsRange(linksToRemove);

                if (menuIdsToAdd.Count > 0)
                {
                    var newLinks = menuIdsToAdd.Select(menuId => new MenuNews { MenuId = menuId, NewsId = news.Id });
                    _newsRepository.AddMenuNewsRange(newLinks);
                }

                await _unitOfWork.CommitAsync(ct);

                var messageParts = new List<string>();
                messageParts.Add(invalidMenuIds.Count > 0
                    ? $"Cập nhật news thành công. {invalidMenuIds.Count} MenuId không hợp lệ đã bị bỏ qua."
                    : "Cập nhật news thành công.");
                if (wardNotFound)
                    messageParts.Add("WardId không tồn tại, giữ nguyên địa chỉ cũ.");

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