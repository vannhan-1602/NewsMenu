using Application.DTOs;
using Application.Request.Menu;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class GetMenuByIdUseCase : IRequestHandler<GetMenuByIdRequest, MenuDto?>
    {
        private readonly IMenuRepository _menuRepository;

        public GetMenuByIdUseCase(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<MenuDto?> Handle(GetMenuByIdRequest request, CancellationToken ct)
        {
            return await _menuRepository.Query()
                .Where(menu => menu.Id == request.Id)
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
                .FirstOrDefaultAsync(ct);
        }
    }
}
