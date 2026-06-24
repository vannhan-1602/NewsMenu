using Application.Common;
using Application.Request.Menu;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class DeleteMenuUseCase : IRequestHandler<DeleteMenuRequest, BaseResponse>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMenuUseCase(IMenuRepository menuRepository, IUnitOfWork unitOfWork)
        {
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(DeleteMenuRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var menu = await _menuRepository.GetByIdAsync(request.Id, ct);
                if (menu == null)
                    return new BaseResponse { Success = false, Message = "Menu khong ton tai hoac da bi xoa" };

              
                menu.IsDeleted = true;
                _menuRepository.Update(menu);

               
                var links = await _menuRepository.GetMenuNewsByMenuIdAsync(menu.Id, ct);
                if (links.Count > 0)
                    _menuRepository.RemoveMenuNewsRange(links);

                await _unitOfWork.CommitAsync(ct);

                return new BaseResponse { Success = true, Message = "Xoa Menu thanh cong", Id = menu.Id };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}
