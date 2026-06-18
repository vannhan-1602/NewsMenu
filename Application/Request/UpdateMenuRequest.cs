using Application.Common;
using MediatR;


namespace Application.Request
{
    public class UpdateMenuRequest : IRequest<BaseResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public List<int> NewsIds { get; set; } = new();
    }
}
