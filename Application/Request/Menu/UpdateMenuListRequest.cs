using Application.Common;
using MediatR;

namespace Application.Request.Menu
{
    public class MenuUpdateItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public List<int> NewsIds { get; set; } = new();
    }

    // Sửa nhiều Menu trong 1 request - xử lý trong 1 transaction duy nhất
    public class UpdateMenuListRequest : IRequest<BaseResponse>
    {
        public List<MenuUpdateItem> Items { get; set; } = new();
    }
}
