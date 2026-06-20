using Application.DTOs;
using MediatR;


namespace Application.Request.News
{
    public class GetNewsByIdRequest : IRequest<NewsDto?>
    {
        public int Id { get; set; }

        public GetNewsByIdRequest(int id) => Id = id;
    }
}
