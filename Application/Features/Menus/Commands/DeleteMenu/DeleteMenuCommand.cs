using MediatR;
using System;

namespace Application.Features.Menus.Commands.DeleteMenu
{
    public record DeleteMenuCommand(Guid MenuId) : IRequest<bool>;
}