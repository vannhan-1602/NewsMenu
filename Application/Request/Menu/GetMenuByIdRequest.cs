using Application.DTOs;
using MediatR;


namespace Application.Request.Menu
{
    public class GetMenuByIdRequest : IRequest<MenuDto?>
    {
        public int Id { get; set; }

        public GetMenuByIdRequest(int id) => Id = id;
    }
}
