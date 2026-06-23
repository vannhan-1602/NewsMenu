namespace Domain.Entities
{
    public class News : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public bool IsPublished { get; set; }

        
        public int? WardId { get; set; }
        public Ward? Ward { get; set; }
        public string? Address { get; set; } 


        public ICollection<MenuNews> MenuNewsList { get; set; } = new List<MenuNews>();
    }
}
