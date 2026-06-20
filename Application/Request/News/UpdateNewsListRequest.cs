using Application.Common;
using MediatR;

namespace Application.Request.News
{
    public class NewsUpdateItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public bool IsPublished { get; set; }
        public List<int> MenuIds { get; set; } = new();
    }

    public class UpdateNewsListRequest : IRequest<BaseResponse>
    {
        public List<NewsUpdateItem> Items { get; set; } = new();
    }
}
