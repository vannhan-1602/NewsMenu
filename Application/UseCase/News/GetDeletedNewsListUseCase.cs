using Application.DTOs;
using Application.Request.News;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                .OrderByDescending(news => news.UpdatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(news => new NewsDto
                {
                    Id = news.Id,
                    Title = news.Title,
                    Content = news.Content,
                    Summary = news.Summary,
                    IsPublished = news.IsPublished,
                    CreatedAt = news.CreatedAt,
                    UpdatedAt = news.UpdatedAt,
                    Address = news.Address,
                    WardId = news.WardId
                })
                .AsAsyncEnumerable();

            return Task.FromResult(result);
        }
    }
}
