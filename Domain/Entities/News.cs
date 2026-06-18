using System.Collections.Generic;

namespace Domain.Entities
{
    public class News : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public bool IsPublished { get; set; }
    }
}
