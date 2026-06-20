using Application.Common;
using MediatR;

namespace Application.Request.News
{
    public class DeleteNewsListRequest : IRequest<BaseResponse>
    {
        public List<int> Ids { get; set; } = new();
    }
}
