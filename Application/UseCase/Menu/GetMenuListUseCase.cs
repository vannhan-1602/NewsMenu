using Application.DTOs;
using Application.Request.Menu;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class GetMenuListUseCase : IRequestHandler<GetMenuListRequest, IAsyncEnumerable<MenuDto>>
    {
        private readonly IMenuRepository _menuRepository;

        public GetMenuListUseCase(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public Task<IAsyncEnumerable<MenuDto>> Handle(GetMenuListRequest request, CancellationToken ct)
        {
            var result = _menuRepository.Query()
                .OrderBy(menu => menu.DisplayOrder)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(menu => new MenuDto
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Slug = menu.Slug,
                    DisplayOrder = menu.DisplayOrder,
                    CreatedAt = menu.CreatedAt,
                    News = menu.MenuNewsList
                        .Where(menuNews => !menuNews.News.IsDeleted)
                        .Select(menuNews => new NewsSimpleDto
                        {
                            Id = menuNews.News.Id,
                            Title = menuNews.News.Title,
                            IsPublished = menuNews.News.IsPublished
                        }).ToArray()
                })
                .AsAsyncEnumerable();

            return Task.FromResult(result);
        }
    }
}
