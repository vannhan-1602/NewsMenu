using Application.Common;
using Application.Request.Menu;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public UpdateMenuUseCase(
            IMenuRepository menuRepository,
            INewsRepository newsRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _menuRepository = menuRepository;
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> Handle(UpdateMenuRequest request, CancellationToken ct)
        {
            // Validate news ids 
            var validNewsIds = new List<int>();
            var invalidNewsIds = new List<int>();
            if (request.NewsIds.Count > 0)
            {
                var existingNewsIds = await _newsRepository.GetExistingIdsAsync(request.NewsIds, ct);
                var existingNewsIdSet = new HashSet<int>(existingNewsIds);
                validNewsIds = existingNewsIds;
                invalidNewsIds = request.NewsIds.Distinct().Where(id => !existingNewsIdSet.Contains(id)).ToList();
            }

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var menu = await _menuRepository.GetByIdAsync(request.Id, ct);
                if (menu == null)
                    return new BaseResponse { Success = false, Message = "Menu không tồn tại hoặc đã bị xóa." };

                _mapper.Map(request, menu);
                _menuRepository.Update(menu);

                // chỉ xóa link không còn dùng, chỉ thêm link mới
                var currentLinks = await _menuRepository.GetMenuNewsByMenuIdAsync(menu.Id, ct);
                var currentNewsIdSet = currentLinks.Select(link => link.NewsId).ToHashSet();
                var newNewsIdSet = new HashSet<int>(validNewsIds);

                var linksToRemove = currentLinks.Where(link => !newNewsIdSet.Contains(link.NewsId)).ToList();
                var newsIdsToAdd = validNewsIds.Where(newsId => !currentNewsIdSet.Contains(newsId)).ToList();

                if (linksToRemove.Count > 0)
                    _menuRepository.RemoveMenuNewsRange(linksToRemove);

                if (newsIdsToAdd.Count > 0)
                {
                    var newLinks = newsIdsToAdd.Select(newsId => new MenuNews { MenuId = menu.Id, NewsId = newsId });
                    _menuRepository.AddMenuNewsRange(newLinks);
                }

                await _unitOfWork.CommitAsync(ct);

                var message = invalidNewsIds.Count > 0
                    ? $"Cập nhật menu thành công. {invalidNewsIds.Count} NewsId không hợp lệ đã bị bỏ qua."
                    : "Cập nhật menu thành công.";

                return new BaseResponse { Success = true, Message = message, Id = menu.Id, InvalidIds = invalidNewsIds };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}