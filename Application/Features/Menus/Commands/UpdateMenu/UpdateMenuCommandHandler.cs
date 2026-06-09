using MediatR;
using Application.Events;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Menus.Commands.UpdateMenu
{
    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<UpdateMenuCommandHandler> _logger;

        public UpdateMenuCommandHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<UpdateMenuCommandHandler> logger)
        {
            _uow = uow; _eventPublisher = eventPublisher; _logger = logger;
        }

        public async Task<bool> Handle(UpdateMenuCommand request, CancellationToken ct)
        {
            var menu = await _uow.Menus.GetByIdAsync(request.MenuId, ct);
            if (menu == null) throw new Exception($"Không tìm thấy Menu với ID {request.MenuId}");

            menu.Name = request.Name;
            menu.Slug = request.Slug;
            menu.DisplayOrder = request.DisplayOrder;
            menu.UpdatedAt = DateTime.UtcNow;

            try
            {
                _uow.Menus.Update(menu);
                await _uow.CommitAsync(ct);

                var @event = new MenuUpdatedEvent(menu.Id, menu.Name, menu.Slug, menu.DisplayOrder);
                await _eventPublisher.PublishAsync("menu.updated", @event, ct);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật Menu");
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}