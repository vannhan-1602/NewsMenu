using Application.DTOs;
using MediatR;

namespace Application.Request
{
    public class GetMenuListRequest : IRequest<List<MenuDto>>
    {
        
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
