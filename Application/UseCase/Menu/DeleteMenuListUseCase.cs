using Application.Common;
using Application.Request.Menu;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class DeleteMenuListUseCase : IRequestHandler<DeleteMenuListRequest, BaseResponse>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMenuListUseCase(IMenuRepository menuRepository, IUnitOfWork unitOfWork)
        {
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(DeleteMenuListRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                // Lấy danh sách Ids duy nhất từ request
                var ids = request.Ids.Distinct().ToArray();

                // Lấy danh sách menu từ cơ sở dữ liệu dựa trên danh sách Ids
                var menuList = new List<Menu>();
                await foreach (var menu in _menuRepository.Query()
                    .Where(menu => ids.Contains(menu.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    menuList.Add(menu);
                }

                var foundIds = menuList.Select(menu => menu.Id).ToHashSet();
                var notFoundCount = ids.Count(id => !foundIds.Contains(id));

                // Batch load toàn bộ links của tất cả menu cần xóa, tránh N+1 query
                var allLinksToRemove = await _menuRepository.GetMenuNewsByMenuIdsAsync(foundIds, ct);

                // Đánh dấu xóa mềm cho tất cả menu
                foreach (var menu in menuList)
                    menu.IsDeleted = true;

                // Cập nhật trạng thái IsDeleted của các menu
                if (menuList.Count > 0)
                    _menuRepository.UpdateRange(menuList);
                // Xóa các liên kết MenuNews
                if (allLinksToRemove.Count > 0)
                    _menuRepository.RemoveMenuNewsRange(allLinksToRemove);

                await _unitOfWork.CommitAsync(ct);

                var message = notFoundCount > 0
                    ? $"Xóa {menuList.Count} menu thành công. {notFoundCount} Id không tồn tại."
                    : $"Xóa {menuList.Count} menu thành công.";

                return new BaseResponse { Success = true, Message = message };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}
