using Application.DTOs;
using Application.Request.Menu;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class GetDeletedMenuListUseCase : IRequestHandler<GetDeletedMenuListRequest, IAsyncEnumerable<MenuDto>>
    {
        private readonly IMenuRepository _menuRepository;

        public GetDeletedMenuListUseCase(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public Task<IAsyncEnumerable<MenuDto>> Handle(GetDeletedMenuListRequest request, CancellationToken ct)
        {
            var result = _menuRepository.QueryDeleted()
                .OrderByDescending(menu => menu.UpdatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(menu => new MenuDto
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Slug = menu.Slug,
                    DisplayOrder = menu.DisplayOrder,
                    CreatedAt = menu.CreatedAt
                })
                .AsAsyncEnumerable();

            return Task.FromResult(result);
        }
    }
}
