using Application.DTOs;
using MediatR;

namespace Application.Request.Menu
{
    public class GetDeletedMenuListRequest : IRequest<IAsyncEnumerable<MenuDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}