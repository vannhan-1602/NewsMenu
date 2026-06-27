using Application.DTOs;
using Application.Request.News;
using Domain.Interfaces;
using MediatR;

namespace Application.UseCase
{
    public class GetDeletedNewsListUseCase : IRequestHandler<GetDeletedNewsListRequest, IAsyncEnumerable<NewsDto>>
    {
        private readonly INewsRepository _newsRepository;

        public GetDeletedNewsListUseCase(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public Task<IAsyncEnumerable<NewsDto>> Handle(GetDeletedNewsListRequest request, CancellationToken ct)
        {
            var result = _newsRepository.QueryDeleted()
                .OrderByDescending(n => n.UpdatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Summary = n.Summary,
                    IsPublished = n.IsPublished,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    Address = n.Address,
                    WardId = n.WardId
                })
                .AsAsyncEnumerable();

            return Task.FromResult(result);
        }
    }
}