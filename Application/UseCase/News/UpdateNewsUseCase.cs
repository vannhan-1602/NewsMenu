using Application.Common;
using Application.Request.News;
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

        public UpdateNewsUseCase(
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

        public async Task<BaseResponse> Handle(UpdateNewsRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {   // Lấy news từ cơ sở dữ liệu
                var news = await _newsRepository.GetByIdAsync(request.Id, ct);
                if (news == null)
                    return new BaseResponse { Success = false, Message = "News khong ton tai hoac da bi xoa" };
                // Cập nhật các thuộc tính của news
                news.Title = request.Title;
                news.Content = request.Content;
                news.Summary = request.Summary;
                news.IsPublished = request.IsPublished;
                news.Address = request.Address;
                // Xử lý WardId
                bool wardInvalid = false;
                if (request.WardId.HasValue)
                {
                    var ward = await _wardRepository.GetByIdAsync(request.WardId.Value, ct);
                    if (ward != null)
                    {
                        news.WardId = ward.Id;
                    }
                    else
                    {
                        // Nếu không tồn tại thì giữ nguyên WardId cũ
                        wardInvalid = true;
                    }
                }
                else
                {
                    // Nếu client truyền null thì xóa thông tin địa chỉ
                    news.WardId = null;
                }

                _newsRepository.Update(news);

                var invalidIds = new List<int>();
                var existingIds = new List<int>();
               
                if (request.MenuIds.Count > 0)
                {
                    // Lấy danh sách MenuId hợp lệ từ cơ sở dữ liệu
                    existingIds = await _menuRepository.GetExistingIdsAsync(request.MenuIds, ct);
                    var existingSet = new HashSet<int>(existingIds);

                    invalidIds = request.MenuIds
                        .Distinct()
                        .Where(id => !existingSet.Contains(id))
                        .ToList();
                }

                //lấy danh sách liên kết MenuNews hiện tại của news
                var currentLinks = await _newsRepository.GetMenuNewsByNewsIdAsync(news.Id, ct);
                // Xóa các liên kết MenuNews hiện tại
                if (currentLinks.Count > 0)
                    _newsRepository.RemoveMenuNewsRange(currentLinks);
                // Thêm các liên kết MenuNews mới
                if (existingIds.Count > 0)
                {
                    var newLinks = existingIds.Select(menuId => new MenuNews { MenuId = menuId, NewsId = news.Id });
                    _newsRepository.AddMenuNewsRange(newLinks);
                }

                await _unitOfWork.CommitAsync(ct);

                var parts = new List<string>();
                parts.Add(invalidIds.Count > 0
                    ? $"Cap nhat news thanh cong. {invalidIds.Count} MenuId khong hop le da bi bo qua."
                    : "Cap nhat news thanh cong");
                if (wardInvalid)
                {
                    parts.Add("WardId khong ton tai, giu nguyen dia chi cu.");
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
