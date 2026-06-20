namespace Domain.Entities
{
    public class News : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public bool IsPublished { get; set; }

        // Bài viết được viết ở địa chỉ nào - WardId trỏ tới Phường/Xã (hoặc Tỉnh/TP nếu không rõ phường)
        public int? WardId { get; set; }
        public Ward? Ward { get; set; }
        public string? Address { get; set; }   // Phần chi tiết không thuộc cây địa danh, vd "Số 1, ngõ ABC"

        // Navigation property - chiều ngược lại với Menu, dùng chung 1 bảng trung gian MenuNews
        public ICollection<MenuNews> MenuNewsList { get; set; } = new List<MenuNews>();
    }
}
