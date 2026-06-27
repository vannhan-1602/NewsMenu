using Application.DTOs;
using MediatR;

namespace Application.Request.News
{
    public class GetDeletedNewsListRequest : IRequest<IAsyncEnumerable<NewsDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
