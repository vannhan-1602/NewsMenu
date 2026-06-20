namespace Application.Common
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Id { get; set; }

        // Id của các quan hệ (NewsIds/MenuIds) không tồn tại trong DB, đã bị bỏ qua khi gán
        public List<int> InvalidIds { get; set; } = new();
    }

    // Response riêng cho thao tác bulk - mỗi item trong Items là kết quả của 1 record trong request
    public class BulkResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<BaseResponse> Items { get; set; } = new();
    }
}
