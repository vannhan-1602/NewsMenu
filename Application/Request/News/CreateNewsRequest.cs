using Application.Common;
using MediatR;

namespace Application.Request.News
{
    public class CreateNewsRequest : IRequest<BaseResponse>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public bool IsPublished { get; set; }
        public List<int> MenuIds { get; set; } = new();
        public int? WardId { get; set; }
        public string? Address { get; set; }
    }
}
