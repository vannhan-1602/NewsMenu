namespace Application.DTOs
{
    
    public class WardInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public WardParentDto? WardParent { get; set; }
        public CountryDto? Country { get; set; }
    }

    public class WardParentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
