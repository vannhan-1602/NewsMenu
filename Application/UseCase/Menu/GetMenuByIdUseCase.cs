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
            // 1 câu Select duy nhất - nhờ navigation property (FK), EF tự JOIN MenuNews -> News
            // ngay trong cùng 1 câu SQL, không cần query phụ như bản trước
            return await _menuRepository.Query()
                .Where(m => m.Id == request.Id)
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
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
