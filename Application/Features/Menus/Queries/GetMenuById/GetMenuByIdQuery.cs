using MediatR;
using Application.DTOs;
using System;

namespace Application.Features.Menus.Queries.GetMenuById
{
    public record GetMenuByIdQuery(Guid MenuId) : IRequest<MenuDto?>;
}