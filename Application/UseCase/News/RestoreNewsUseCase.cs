using Application.Common;
using Application.Request.News;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class RestoreNewsUseCase : IRequestHandler<RestoreNewsRequest, BaseResponse>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RestoreNewsUseCase(INewsRepository newsRepository, IUnitOfWork unitOfWork)
        {
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(RestoreNewsRequest request, CancellationToken ct)
        {
            var news = await _newsRepository.GetByIdIncludeDeletedAsync(request.Id, ct);
            if (news == null)
                return new BaseResponse { Success = false, Message = "News không tồn tại." };

            if (!news.IsDeleted)
                return new BaseResponse { Success = false, Message = "News chưa bị xóa, không cần khôi phục." };

            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                _newsRepository.Restore(news);
                await _unitOfWork.CommitAsync(ct);
                return new BaseResponse { Success = true, Message = "Khôi phục News thành công.", Id = news.Id };
            }
            catch
            {
                await _unitOfWork.RollbackAsync(ct);
                throw;
            }
        }
    }
}