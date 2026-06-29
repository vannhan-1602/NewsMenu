using Application.Common;
using Application.Request.News;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class DeleteNewsListUseCase : IRequestHandler<DeleteNewsListRequest, BaseResponse>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNewsListUseCase(INewsRepository newsRepository, IUnitOfWork unitOfWork)
        {
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> Handle(DeleteNewsListRequest request, CancellationToken ct)
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                // Lấy danh sách Ids duy nhất từ request
                var ids = request.Ids.Distinct().ToArray();
                var newsList = new List<News>();
                await foreach (var news in _newsRepository.Query()
                    .Where(news => ids.Contains(news.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    newsList.Add(news);
                }

                var foundIds = newsList.Select(news => news.Id).ToHashSet();
                var notFoundCount = ids.Count(id => !foundIds.Contains(id));

                // Batch load toàn bộ links của tất cả news cần xóa, tránh N+1 query
                var allLinksToRemove = await _newsRepository.GetMenuNewsByNewsIdsAsync(foundIds, ct);

                // Đánh dấu xóa mềm cho tất cả news
                foreach (var news in newsList)
                    news.IsDeleted = true;

                // Cập nhật trạng thái IsDeleted cho các News
                if (newsList.Count > 0)
                    _newsRepository.UpdateRange(newsList);
                // Xóa các liên kết MenuNews
                if (allLinksToRemove.Count > 0)
                    _newsRepository.RemoveMenuNewsRange(allLinksToRemove);

                await _unitOfWork.CommitAsync(ct);

                var message = notFoundCount > 0
                    ? $"Xóa {newsList.Count} news thành công. {notFoundCount} Id không tồn tại."
                    : $"Xóa {newsList.Count} news thành công.";

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
