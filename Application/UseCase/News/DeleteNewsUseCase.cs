using Application.Common;
using Application.Request.News;
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
                // Lấy news từ cơ sở dữ liệu
                var news = await _newsRepository.GetByIdAsync(request.Id, ct);
                if (news == null)
                    return new BaseResponse { Success = false, Message = "News khong ton tai hoac da bi xoa" };

                news.IsDeleted = true;
                _newsRepository.Update(news);

                // Không xóa liên kết MenuNews khi soft-delete

                await _unitOfWork.CommitAsync(ct);

                return new BaseResponse { Success = true, Message = "Xoa News thanh cong", Id = news.Id };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}