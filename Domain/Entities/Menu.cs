namespace Domain.Entities
{
    public class Menu : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        // Navigation property - EF Core dùng để JOIN qua bảng trung gian MenuNews
        // Nhờ có FK thật, có thể viết m.MenuNewsList.Select(x => x.News...) ngay trong Select
        public ICollection<MenuNews> MenuNewsList { get; set; } = new List<MenuNews>();
    }
}
