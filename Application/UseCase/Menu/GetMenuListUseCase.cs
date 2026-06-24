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
                .OrderBy(m => m.DisplayOrder)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Slug = m.Slug,
                    DisplayOrder = m.DisplayOrder,
                    CreatedAt = m.CreatedAt,
                    News = m.MenuNewsList
                        .Where(mn => !mn.News.IsDeleted)
                        .Select(mn => new NewsSimpleDto
                        {
                            Id = mn.News.Id,
                            Title = mn.News.Title,
                            IsPublished = mn.News.IsPublished
                        }).ToArray()
                })
                .AsAsyncEnumerable();

            return Task.FromResult(result);
        }
    }
}
