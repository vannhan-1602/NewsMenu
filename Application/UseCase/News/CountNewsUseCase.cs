using Application.Request.News;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class CountNewsUseCase : IRequestHandler<CountNewsRequest, int>
    {
        private readonly INewsRepository _newsRepository;

        public CountNewsUseCase(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public Task<int> Handle(CountNewsRequest request, CancellationToken ct)
        {
            var query = _newsRepository.Query();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
                query = query.Where(news => news.Title.Contains(request.Keyword));

            if (request.IsPublished.HasValue)
                query = query.Where(news => news.IsPublished == request.IsPublished.Value);

            if (request.MenuId.HasValue)
                query = query.Where(news => news.MenuNewsList.Any(menuNews => menuNews.MenuId == request.MenuId.Value));

            if (request.DateFrom.HasValue)
                query = query.Where(news => news.CreatedAt >= request.DateFrom.Value);

            if (request.DateTo.HasValue)
                query = query.Where(news => news.CreatedAt <= request.DateTo.Value);

            return query.CountAsync(ct);
        }
    }
}
