using Application.Common;
using Application.Request;
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
                    return new BaseResponse { Success = false, Message = "Menu không tồn tại hoặc đã bị xóa" };

                // Xóa liên kết bảng trung gian trước khi soft delete
                await _menuRepository.RemoveMenuNewsByMenuIdAsync(menu.Id, ct);
                _menuRepository.SoftDelete(menu);

                await _unitOfWork.CommitAsync(ct);

                
                return new BaseResponse { Success = true, Message = "Xóa Menu thành công", Id = menu.Id };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}