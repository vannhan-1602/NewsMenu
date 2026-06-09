using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Menus.Queries.GetMenuList
{
    public record GetMenuListQuery() : IRequest<IEnumerable<MenuDto>>;
}