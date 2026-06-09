using MediatR;
using Application.Events;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.News.Commands.UpdateNews
{
    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEventPublisher _eventPublisher;

        public UpdateNewsCommandHandler(IUnitOfWork uow, IEventPublisher eventPublisher)
        {
            _uow = uow; _eventPublisher = eventPublisher;
        }

        public async Task<bool> Handle(UpdateNewsCommand request, CancellationToken ct)
        {
            var news = await _uow.News.GetWithMenusAsync(request.NewsId, ct);
            if (news == null) throw new Exception("Không tìm thấy News");

            news.Title = request.Title;
            news.Content = request.Content;
            news.Summary = request.Summary;
            news.IsPublished = request.IsPublished;

            // Cập nhật quan hệ MenuNews
            news.MenuNews.Clear();
            foreach (var menuId in request.MenuIds)
            {
                news.MenuNews.Add(new Domain.Entities.MenuNews { MenuId = menuId, NewsId = news.Id, AssignedAt = DateTime.UtcNow });
            }

            _uow.News.Update(news);
            await _uow.CommitAsync(ct);

            await _eventPublisher.PublishAsync("news.updated", new NewsUpdatedEvent(news.Id, news.Title, news.Content, news.Summary, news.IsPublished, request.MenuIds), ct);
            return true;
        }
    }
}