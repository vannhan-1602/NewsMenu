using MediatR;
using Application.Events;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Menus.Commands.DeleteMenu
{
    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<DeleteMenuCommandHandler> _logger;

        public DeleteMenuCommandHandler(IUnitOfWork uow, IEventPublisher eventPublisher, ILogger<DeleteMenuCommandHandler> logger)
        {
            _uow = uow; _eventPublisher = eventPublisher; _logger = logger;
        }

        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken ct)
        {
            var menu = await _uow.Menus.GetByIdAsync(request.MenuId, ct);
            if (menu == null) throw new Exception($"Không tìm thấy Menu với ID {request.MenuId}");

            try
            {
                _uow.Menus.SoftDelete(menu);
                await _uow.CommitAsync(ct);

                await _eventPublisher.PublishAsync("menu.deleted", new MenuDeletedEvent(menu.Id), ct);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa Menu");
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}