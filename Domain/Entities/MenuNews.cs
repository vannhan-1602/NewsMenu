namespace Domain.Entities
{
    // Bảng trung gian (junction table) cho quan hệ N-N giữa Menu và News
    public class MenuNews
    {
        public int MenuId { get; set; }
        public int NewsId { get; set; }
        public DateTime AssignedAt { get; set; }

        // Navigation property 2 chiều - EF Core dùng để tự JOIN khi truy vấn
        // Đây là phần "khóa ngoại trong code" thay vì trong DB
        public Menu Menu { get; set; } = null!;
        public News News { get; set; } = null!;
    }
}
