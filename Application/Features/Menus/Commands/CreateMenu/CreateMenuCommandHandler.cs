using MediatR;
using Application.Events;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Menus.Commands.CreateMenu
{
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<CreateMenuCommandHandler> _logger;

        public CreateMenuCommandHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<CreateMenuCommandHandler> logger)
        {
            _uow = uow; _eventPublisher = eventPublisher; _logger = logger;
        }

        public async Task<Guid> Handle(CreateMenuCommand request, CancellationToken ct)
        {
            var menu = new Domain.Entities.Menu
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug,
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                await _uow.Menus.AddAsync(menu, ct);
                await _uow.CommitAsync(ct);
                var @event = new MenuCreatedEvent(menu.Id, menu.Name, menu.Slug, menu.DisplayOrder);
                await _eventPublisher.PublishAsync("menu.created", @event, ct);

                return menu.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo Menu");
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}