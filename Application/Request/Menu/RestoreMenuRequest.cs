using Application.Common;
using MediatR;

namespace Application.Request.Menu
{
    public class RestoreMenuRequest : IRequest<BaseResponse>
    {
        public int Id { get; set; }
    }
}