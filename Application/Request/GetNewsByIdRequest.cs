using Application.DTOs;
using MediatR;


namespace Application.Request
{
    public class GetNewsByIdRequest : IRequest<NewsDto?>
    {
        public int Id { get; set; }

        public GetNewsByIdRequest(int id) => Id = id;
    }
}
