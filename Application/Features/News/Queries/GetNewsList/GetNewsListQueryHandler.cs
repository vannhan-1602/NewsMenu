using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOs;
using Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Application.Features.News.Queries.GetNewsList
{
    public class GetNewsListQueryHandler : IRequestHandler<GetNewsListQuery, IEnumerable<NewsDto>>
    {
        private readonly INewsReadRepository _readRepo;

        public GetNewsListQueryHandler(INewsReadRepository readRepo)
        {
            _readRepo = readRepo;
        }

        public async Task<IEnumerable<NewsDto>> Handle(GetNewsListQuery request, CancellationToken ct)
        {
            return await _readRepo.GetAllAsync(request.Page, request.PageSize, ct);
        }
    }
}