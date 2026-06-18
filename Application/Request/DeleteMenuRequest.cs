using Application.Common;
using MediatR;


namespace Application.Request
{
    public class DeleteMenuRequest : IRequest<BaseResponse>
    {
        public int Id { get; set; }

        public DeleteMenuRequest(int id) => Id = id;
    }
}
