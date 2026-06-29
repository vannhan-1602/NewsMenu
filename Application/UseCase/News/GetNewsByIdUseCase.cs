using Application.DTOs;
using Application.Request.News;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class GetNewsByIdUseCase : IRequestHandler<GetNewsByIdRequest, NewsDto?>
    {
        private readonly INewsRepository _newsRepository;

        public GetNewsByIdUseCase(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<NewsDto?> Handle(GetNewsByIdRequest request, CancellationToken ct)
        {
            return await _newsRepository.Query()
                .Where(news => news.Id == request.Id)
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
                            DisplayOrder = menuNews.Menu.DisplayOrder
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
                .FirstOrDefaultAsync(ct);
        }
    }
}
