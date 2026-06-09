using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.News.Queries.GetNewsList
{
    public record GetNewsListQuery(int Page, int PageSize) : IRequest<IEnumerable<NewsDto>>;
}