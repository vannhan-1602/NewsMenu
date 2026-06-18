using Application.DTOs;
using Application.Request;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class GetNewsListUseCase : IRequestHandler<GetNewsListRequest, List<NewsDto>>
    {
        private readonly INewsRepository _newsRepository;

        public GetNewsListUseCase(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<List<NewsDto>> Handle(GetNewsListRequest request, CancellationToken ct)
        {
         
            var newsList = await _newsRepository.Query()
                .OrderByDescending(n => n.CreatedAt)
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
                    UpdatedAt = n.UpdatedAt
                })
                .ToListAsync(ct);

            if (!newsList.Any()) return newsList;

            
            var newsIds = newsList.Select(n => n.Id).ToList();
            var allMenus = await _newsRepository.GetMenusOfNewsListAsync(newsIds, ct);

          
            var menusByNewsId = allMenus
                .GroupBy(x => x.NewsId)
                .ToDictionary(g => g.Key, g => g.Select(x => new MenuDto
                {
                    Id = x.MenuId,
                    Name = x.Name,
                    Slug = x.Slug,
                    DisplayOrder = x.DisplayOrder
                }).ToList());

            foreach (var news in newsList)
                news.Menus = menusByNewsId.GetValueOrDefault(news.Id) ?? new List<MenuDto>();

            return newsList;
        }
    }
}
