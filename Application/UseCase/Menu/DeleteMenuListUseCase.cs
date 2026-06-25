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
                var ids = request.Ids.Distinct().ToList();

                // Lấy danh sách menu từ cơ sở dữ liệu dựa trên danh sách Ids
                var menus = new List<Menu>();
                await foreach (var menu in _menuRepository.Query()
                    .Where(m => ids.Contains(m.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    menus.Add(menu);
                }
                //tìm id tồn tại trong db
                var foundIds = menus.Select(m => m.Id).ToHashSet();
                //tìm id không tồn tại trong db
                var notFoundCount = ids.Count(id => !foundIds.Contains(id));

                //danh sách MenuNews cần xóa
                var allLinksToRemove = new List<MenuNews>();

                foreach (var menu in menus)
                {
                    menu.IsDeleted = true;
                    // Lấy danh sách MenuNews liên quan đến menu hiện tại
                    var links = await _menuRepository.GetMenuNewsByMenuIdAsync(menu.Id, ct);
                    allLinksToRemove.AddRange(links);
                }
                // Cập nhật trạng thái IsDeleted của các menu
                if (menus.Count > 0)
                    _menuRepository.UpdateRange(menus);
                // Xóa các liên kết MenuNews
                if (allLinksToRemove.Count > 0)
                    _menuRepository.RemoveMenuNewsRange(allLinksToRemove);

                await _unitOfWork.CommitAsync(ct);

                var message = notFoundCount > 0
                    ? $"Xoa {menus.Count} menu thanh cong. {notFoundCount} Id khong ton tai."
                    : $"Xoa {menus.Count} menu thanh cong";

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
