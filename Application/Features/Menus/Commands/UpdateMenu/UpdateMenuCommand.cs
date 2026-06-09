using MediatR;
using System;

namespace Application.Features.Menus.Commands.UpdateMenu
{
    public record UpdateMenuCommand(Guid MenuId, string Name, string Slug, int DisplayOrder) : IRequest<bool>;
}