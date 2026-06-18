using Application.DTOs;
using Application.Request;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class GetNewsByIdUseCase : IRequestHandler<GetNewsByIdRequest, NewsDto?>
    {
        private readonly INewsRepository _newsRepository;

        public GetNewsByIdUseCase(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<NewsDto?> Handle(GetNewsByIdRequest request, CancellationToken ct)
        {
            
            var news = await _newsRepository.Query()
                .Where(n => n.Id == request.Id)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Summary = n.Summary,
                    IsPublished = n.IsPublished,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt
                })
                .FirstOrDefaultAsync(ct);

            if (news == null) return null;

            
            var menus = await _newsRepository.GetMenusOfNewsAsync(news.Id, ct);
            news.Menus = menus.Select(m => new MenuDto
            {
                Id = m.MenuId,
                Name = m.Name,
                Slug = m.Slug,
                DisplayOrder = m.DisplayOrder
            }).ToList();

            return news;
        }
    }
}
