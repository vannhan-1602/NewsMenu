using Application.DTOs;
using MediatR;

namespace Application.Request.News
{
    public enum NewsSortBy
    {
        CreatedAtDesc = 0,
        CreatedAtAsc = 1,
        TitleAsc = 2,
        TitleDesc = 3,
        UpdatedAtDesc = 4
    }

    public class GetNewsListRequest : IRequest<IAsyncEnumerable<NewsDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Tìm theo Title (LIKE %keyword%)
        public string? Keyword { get; set; }

        // Lọc theo trạng thái publish, null = lấy cả 2
        public bool? IsPublished { get; set; }

        // Lọc theo Menu đang chứa News
        public int? MenuId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public NewsSortBy SortBy { get; set; } = NewsSortBy.CreatedAtDesc;
    }
}
