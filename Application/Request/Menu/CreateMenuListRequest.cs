using Application.Common;
using MediatR;

namespace Application.Request.Menu
{
    public class MenuCreateItem
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public List<int> NewsIds { get; set; } = new();
    }

    // Thêm nhiều Menu trong 1 request 
    public class CreateMenuListRequest : IRequest<BaseResponse>
    {
        public List<MenuCreateItem> Items { get; set; } = new();
    }
}
