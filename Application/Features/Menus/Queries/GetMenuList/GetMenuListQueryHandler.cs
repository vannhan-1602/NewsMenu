using MediatR;
using Application.DTOs;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Menus.Queries.GetMenuList
{
    public class GetMenuListQueryHandler : IRequestHandler<GetMenuListQuery, IEnumerable<MenuDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetMenuListQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<IEnumerable<MenuDto>> Handle(GetMenuListQuery request, CancellationToken ct)
        {
            var menus = await _uow.Menus.GetAllAsync(ct);
            return menus.Select(m => new MenuDto(m.Id, m.Name, m.Slug, m.DisplayOrder));
        }
    }
}