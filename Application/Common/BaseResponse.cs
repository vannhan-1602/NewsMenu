namespace Application.Common
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }

        public List<int> InvalidIds { get; set; } = new();
    }

    // Response riêng cho thao tác xử lý hàng loạt
    public class BulkResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<BaseResponse> Items { get; set; } = new();
    }
}
