namespace Application.DTOs
{
    // Chỉ 2 cấp cố định: Ward (Phường/Xã) chứa WardParent (Tỉnh/TP), không đệ quy thêm
    public class WardInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Tên đầy đủ của riêng Ward này (không gồm Address/số nhà) - "Phường Hoàng Mai, Thành phố Hà Nội, Việt Nam"
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
