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
        public MenuDto[] Menus { get; set; } = Array.Empty<MenuDto>();

        public int? WardId { get; set; }
        public string? Address { get; set; }
        public string? FullAddress { get; set; }
        public WardInfoDto? WardInfo { get; set; }
    }
}
