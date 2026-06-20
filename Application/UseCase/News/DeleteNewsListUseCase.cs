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
                var ids = request.Ids.Distinct().ToList();

                // Lấy toàn bộ News cần xóa trong 1 query, không loop GetByIdAsync từng cái
                var newsList = new List<News>();
                await foreach (var news in _newsRepository.Query()
                    .Where(n => ids.Contains(n.Id))
                    .AsAsyncEnumerable()
                    .WithCancellation(ct))
                {
                    newsList.Add(news);
                }

                var foundIds = newsList.Select(n => n.Id).ToHashSet();
                var notFoundCount = ids.Count(id => !foundIds.Contains(id));

                // Xóa mềm = update trạng thái hàng loạt, không có hàm SoftDelete riêng
                var allLinksToRemove = new List<MenuNews>();
                foreach (var news in newsList)
                {
                    news.IsDeleted = true;
                    var links = await _newsRepository.GetMenuNewsByNewsIdAsync(news.Id, ct);
                    allLinksToRemove.AddRange(links);
                }

                if (newsList.Count > 0)
                    _newsRepository.UpdateRange(newsList);
                if (allLinksToRemove.Count > 0)
                    _newsRepository.RemoveMenuNewsRange(allLinksToRemove);

                await _unitOfWork.CommitAsync(ct);

                var message = notFoundCount > 0
                    ? $"Xoa {newsList.Count} news thanh cong. {notFoundCount} Id khong ton tai."
                    : $"Xoa {newsList.Count} news thanh cong";

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
