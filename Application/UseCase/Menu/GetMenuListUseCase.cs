using Application.DTOs;
using Application.Request.Menu;
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
            // 1 câu Select duy nhất - EF tự JOIN qua navigation property (FK đã gắn ở Configuration)
            var query = _menuRepository.Query()
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
                        }).ToList()
                });

            // AsAsyncEnumerable() + await foreach thay cho ToListAsync()
            // EF đọc và trả về từng row một (streaming), không gom hết page vào buffer cùng lúc
            var result = new List<MenuDto>();
            await foreach (var menu in query.AsAsyncEnumerable().WithCancellation(ct))
                result.Add(menu);

            return result;
        }
    }
}
