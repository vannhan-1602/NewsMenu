using Application.DTOs;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.News.Queries.GetNewsById
{
    public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsDto?>
    {
        private readonly INewsReadRepository _readRepo;

        public GetNewsByIdQueryHandler(INewsReadRepository readRepo)
        {
            _readRepo = readRepo;
        }
        public async Task<NewsDto?> Handle(GetNewsByIdQuery request, CancellationToken ct)
            => await _readRepo.GetByIdAsync(request.NewsId, ct);
    }
}
