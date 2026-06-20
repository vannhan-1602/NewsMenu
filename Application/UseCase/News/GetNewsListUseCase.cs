using Application.DTOs;
using Application.Request.News;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase
{
    public class GetNewsListUseCase : IRequestHandler<GetNewsListRequest, List<NewsDto>>
    {
        private readonly INewsRepository _newsRepository;

        public GetNewsListUseCase(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<List<NewsDto>> Handle(GetNewsListRequest request, CancellationToken ct)
        {
            // 1 câu Select duy nhất - EF tự JOIN Menu, Ward, Ward.Parent, Ward.Country qua navigation
            var query = _newsRepository.Query()
                .OrderByDescending(n => n.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Summary = n.Summary,
                    IsPublished = n.IsPublished,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    Address = n.Address,
                    WardId = n.WardId,
                    Menus = n.MenuNewsList
                        .Where(mn => !mn.Menu.IsDeleted)
                        .Select(mn => new MenuDto
                        {
                            Id = mn.Menu.Id,
                            Name = mn.Menu.Name,
                            Slug = mn.Menu.Slug,
                            DisplayOrder = mn.Menu.DisplayOrder
                        }).ToList(),

                    FullAddress = n.Ward == null
                        ? n.Address
                        : (n.Address ?? string.Empty) + ", " + n.Ward.Name
                            + (n.Ward.Parent != null ? ", " + n.Ward.Parent.Name : string.Empty)
                            + ", " + n.Ward.Country.Name,

                    WardInfo = n.Ward == null ? null : new WardInfoDto
                    {
                        Id = n.Ward.Id,
                        Name = n.Ward.Name,
                        FullName = n.Ward.Name
                            + (n.Ward.Parent != null ? ", " + n.Ward.Parent.Name : string.Empty)
                            + ", " + n.Ward.Country.Name,
                        WardParent = n.Ward.Parent == null ? null : new WardParentDto
                        {
                            Id = n.Ward.Parent.Id,
                            Name = n.Ward.Parent.Name
                        },
                        Country = new CountryDto { Id = n.Ward.Country.Id, Name = n.Ward.Country.Name }
                    }
                });

            // AsAsyncEnumerable() + await foreach thay cho ToListAsync()
            var result = new List<NewsDto>();
            await foreach (var news in query.AsAsyncEnumerable().WithCancellation(ct))
                result.Add(news);

            return result;
        }
    }
}
