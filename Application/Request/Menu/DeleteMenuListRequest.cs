using Application.Common;
using MediatR;

namespace Application.Request.Menu
{
    // Xóa nhiều Menu trong 1 request - body: { "ids": [1, 2, 3] }
    public class DeleteMenuListRequest : IRequest<BaseResponse>
    {
        public List<int> Ids { get; set; } = new();
    }
}
