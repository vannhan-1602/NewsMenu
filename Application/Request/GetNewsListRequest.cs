using Application.DTOs;
using MediatR;


namespace Application.Request
{
    public class GetNewsListRequest : IRequest<List<NewsDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
