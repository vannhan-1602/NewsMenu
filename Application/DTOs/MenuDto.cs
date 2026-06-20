namespace Application.DTOs
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<NewsSimpleDto> News { get; set; } = new();
    }

    public class NewsSimpleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
    }
}