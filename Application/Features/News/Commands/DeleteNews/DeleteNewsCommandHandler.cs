using MediatR;
using Application.Events;
using Application.Interfaces;
using Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.News.Commands.DeleteNews
{
    public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEventPublisher _eventPublisher;

        public DeleteNewsCommandHandler(IUnitOfWork uow, IEventPublisher eventPublisher)
        {
            _uow = uow;
            _eventPublisher = eventPublisher;
        }

        public async Task<bool> Handle(DeleteNewsCommand request, CancellationToken ct)
        {
            var news = await _uow.News.GetByIdAsync(request.NewsId, ct);
            if (news == null) throw new Exception("Không tìm thấy News");

            _uow.News.SoftDelete(news);
            await _uow.CommitAsync(ct);

            await _eventPublisher.PublishAsync("news.deleted", new NewsDeletedEvent(news.Id), ct);
            return true;
        }
    }
}