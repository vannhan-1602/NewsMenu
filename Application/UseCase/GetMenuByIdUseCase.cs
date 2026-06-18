using Application.DTOs;
using Application.Request;
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
            
            var menu = await _menuRepository.Query()
                .Where(m => m.Id == request.Id)
                .Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Slug = m.Slug,
                    DisplayOrder = m.DisplayOrder,
                    CreatedAt = m.CreatedAt
                })
                .FirstOrDefaultAsync(ct);

            if (menu == null) return null;

           
            var newsList = await _menuRepository.GetNewsOfMenuAsync(menu.Id, ct);
            menu.News = newsList.Select(n => new NewsSimpleDto
            {
                Id = n.NewsId,
                Title = n.Title,
                IsPublished = n.IsPublished
            }).ToList();

            return menu;
        }
    }
}
