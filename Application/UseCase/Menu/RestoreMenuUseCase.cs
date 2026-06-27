using Application.Common;
using Application.Request.Menu;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class RestoreMenuUseCase : IRequestHandler<RestoreMenuRequest, BaseResponse>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RestoreMenuUseCase(IMenuRepository menuRepository, IUnitOfWork unitOfWork)
        {
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(RestoreMenuRequest request, CancellationToken ct)
        {
            var menu = await _menuRepository.GetByIdIncludeDeletedAsync(request.Id, ct);
            if (menu == null)
                return new BaseResponse { Success = false, Message = "Menu không tồn tại." };

            if (!menu.IsDeleted)
                return new BaseResponse { Success = false, Message = "Menu chưa bị xóa, không cần khôi phục." };

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                _menuRepository.Restore(menu);
                await _unitOfWork.CommitAsync(ct);
                return new BaseResponse { Success = true, Message = "Khôi phục Menu thành công.", Id = menu.Id };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}