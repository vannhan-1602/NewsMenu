using Application.DTOs;
using Application.Request.News;
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
            
            //phép đệ quy 1 cấp (Phường -> Tỉnh/TP)
            return await _newsRepository.Query()
                .Where(n => n.Id == request.Id)
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
                        }).ToArray(),

                    // Cách 1: cộng chuỗi - address + Ward + Ward.Parent (đệ quy) + Country
                    FullAddress = n.Ward == null
                        ? n.Address
                        : (n.Address ?? string.Empty) + ", " + n.Ward.Name
                            + (n.Ward.Parent != null ? ", " + n.Ward.Parent.Name : string.Empty)
                            + ", " + n.Ward.Country.Name,

                    // Cách 2: lồng object qua đệ quy Ward.Parent
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
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
