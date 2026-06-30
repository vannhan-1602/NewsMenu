using MediatR;

namespace Application.Request.News
{
    // Dùng để tính TotalCount cho phân trang, tách riêng khỏi GetNewsListRequest
    public class CountNewsRequest : IRequest<int>
    {
        public string? Keyword { get; set; }
        public bool? IsPublished { get; set; }
        public int? MenuId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public static CountNewsRequest FromListRequest(GetNewsListRequest request) => new()
        {
            Keyword = request.Keyword,
            IsPublished = request.IsPublished,
            MenuId = request.MenuId,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo
        };
    }
}
