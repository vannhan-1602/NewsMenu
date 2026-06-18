using Application.DTOs;
using Application.Request;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class GetMenuListUseCase : IRequestHandler<GetMenuListRequest, List<MenuDto>>
    {
        private readonly IMenuRepository _menuRepository;

        public GetMenuListUseCase(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<List<MenuDto>> Handle(GetMenuListRequest request, CancellationToken ct)
        {
           
            var menuList = await _menuRepository.Query()
                .OrderBy(m => m.DisplayOrder)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Slug = m.Slug,
                    DisplayOrder = m.DisplayOrder,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync(ct);

            if (!menuList.Any()) return menuList;

            
            var menuIds = menuList.Select(m => m.Id).ToList();
            var allNews = await _menuRepository.GetNewsOfMenuListAsync(menuIds, ct);

            
            var newsByMenuId = allNews
                .GroupBy(x => x.MenuId)
                .ToDictionary(g => g.Key, g => g.Select(x => new NewsSimpleDto
                {
                    Id = x.NewsId,
                    Title = x.Title,
                    IsPublished = x.IsPublished
                }).ToList());

            foreach (var menu in menuList)
                menu.News = newsByMenuId.GetValueOrDefault(menu.Id) ?? new List<NewsSimpleDto>();

            return menuList;
        }
    }
}
