namespace Application.DTOs
{
    public class NewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<MenuDto> Menus { get; set; } = new();

        // Bài viết được viết ở địa chỉ nào
        public int? WardId { get; set; }
        public string? Address { get; set; }
        public string? FullAddress { get; set; }     // Cách 1: chuỗi ghép sẵn (address + ward + parent + country)
        public WardInfoDto? WardInfo { get; set; }     // Cách 2: lồng object (Ward + WardParent + Country)
    }
}