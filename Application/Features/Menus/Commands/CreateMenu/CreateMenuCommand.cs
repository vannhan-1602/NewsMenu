using MediatR;
using Application.Events;
using Application.Interfaces;
using Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Menus.Commands.CreateMenu
{
    public record CreateMenuCommand(string Name, string Slug, int DisplayOrder) : IRequest<Guid>;
}