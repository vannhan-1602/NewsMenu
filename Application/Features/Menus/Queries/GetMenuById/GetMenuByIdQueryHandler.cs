using MediatR;
using Application.DTOs;
using Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Menus.Queries.GetMenuById
{
    public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, MenuDto?>
    {
        private readonly IUnitOfWork _uow;

        public GetMenuByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<MenuDto?> Handle(GetMenuByIdQuery request, CancellationToken ct)
        {
            var menu = await _uow.Menus.GetByIdAsync(request.MenuId, ct);
            if (menu == null) return null;

            return new MenuDto(menu.Id, menu.Name, menu.Slug, menu.DisplayOrder);
        }
    }
}