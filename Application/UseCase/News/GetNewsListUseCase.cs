using Application.DTOs;
using Application.Request.News;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class GetNewsListUseCase : IRequestHandler<GetNewsListRequest, IAsyncEnumerable<NewsDto>>
    {
        private readonly INewsRepository _newsRepository;

        public GetNewsListUseCase(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public Task<IAsyncEnumerable<NewsDto>> Handle(GetNewsListRequest request, CancellationToken ct)
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

            query = request.SortBy switch
            {
                NewsSortBy.CreatedAtAsc => query.OrderBy(news => news.CreatedAt),
                NewsSortBy.TitleAsc => query.OrderBy(news => news.Title),
                NewsSortBy.TitleDesc => query.OrderByDescending(news => news.Title),
                NewsSortBy.UpdatedAtDesc => query.OrderByDescending(news => news.UpdatedAt),
                _ => query.OrderByDescending(news => news.CreatedAt)
            };

            var result = query
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
                    WardId = news.WardId,
                    Menus = news.MenuNewsList
                        .Where(menuNews => !menuNews.Menu.IsDeleted)
                        .Select(menuNews => new MenuDto
                        {
                            Id = menuNews.Menu.Id,
                            Name = menuNews.Menu.Name,
                            Slug = menuNews.Menu.Slug,
                            DisplayOrder = menuNews.Menu.DisplayOrder,
                            CreatedAt = menuNews.Menu.CreatedAt
                        }).ToArray(),
                    // Cách 1: cộng chuỗi - address + Ward + Ward.Parent (đệ quy) + Country
                    FullAddress = news.Ward == null
                        ? news.Address
                        : (news.Address ?? string.Empty) + ", " + news.Ward.Name
                            + (news.Ward.Parent != null ? ", " + news.Ward.Parent.Name : string.Empty)
                            + ", " + news.Ward.Country.Name,
                    // Cách 2: lồng object qua đệ quy Ward.Parent
                    WardInfo = news.Ward == null ? null : new WardInfoDto
                    {
                        Id = news.Ward.Id,
                        Name = news.Ward.Name,
                        FullName = news.Ward.Name
                            + (news.Ward.Parent != null ? ", " + news.Ward.Parent.Name : string.Empty)
                            + ", " + news.Ward.Country.Name,
                        WardParent = news.Ward.Parent == null ? null : new WardParentDto
                        {
                            Id = news.Ward.Parent.Id,
                            Name = news.Ward.Parent.Name
                        },
                        Country = new CountryDto { Id = news.Ward.Country.Id, Name = news.Ward.Country.Name }
                    }
                })
                .AsAsyncEnumerable();

            return Task.FromResult(result);
        }
    }
}
