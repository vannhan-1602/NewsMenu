using Application.DTOs;
using MediatR;

namespace Application.Request.Address
{
    public class GetWardListRequest : IRequest<IAsyncEnumerable<WardDto>>
    {
        public int ParentId { get; set; } = 0;
    }
}
