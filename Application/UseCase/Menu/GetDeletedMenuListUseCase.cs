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
                .OrderByDescending(m => m.UpdatedAt)
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
                .AsAsyncEnumerable();

            return Task.FromResult(result);
        }
    }
}