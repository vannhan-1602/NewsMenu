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
                var ids = request.Ids.Distinct().ToList();

               
                var menus = new List<Menu>();
                await foreach (var menu in _menuRepository.Query()
                    .Where(m => ids.Contains(m.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    menus.Add(menu);
                }

                var foundIds = menus.Select(m => m.Id).ToHashSet();
                var notFoundCount = ids.Count(id => !foundIds.Contains(id));

                
                var allLinksToRemove = new List<MenuNews>();
                foreach (var menu in menus)
                {
                    menu.IsDeleted = true;
                    var links = await _menuRepository.GetMenuNewsByMenuIdAsync(menu.Id, ct);
                    allLinksToRemove.AddRange(links);
                }

                if (menus.Count > 0)
                    _menuRepository.UpdateRange(menus);
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
