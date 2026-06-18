using Application.Common;
using MediatR;


namespace Application.Request
{
    public class DeleteNewsRequest : IRequest<BaseResponse>
    {
        public int Id { get; set; }

        public DeleteNewsRequest(int id) => Id = id;
    }
}
