using Application.Common;
using Application.Request;
using Domain.Interfaces;
using MediatR;


namespace Application.UseCase
{
    public class DeleteNewsUseCase : IRequestHandler<DeleteNewsRequest, BaseResponse>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNewsUseCase(INewsRepository newsRepository, IUnitOfWork unitOfWork)
        {
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(DeleteNewsRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var news = await _newsRepository.GetByIdAsync(request.Id, ct);
                if (news == null)
                    return new BaseResponse { Success = false, Message = "News không tồn tại hoặc đã bị xóa" };

                // Xóa liên kết bảng trung gian trước khi soft delete
                await _newsRepository.RemoveMenuNewsByNewsIdAsync(news.Id, ct);
                _newsRepository.SoftDelete(news);

                await _unitOfWork.CommitAsync(ct);

                return new BaseResponse { Success = true, Message = "Xóa News thành công", Id = news.Id };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}
