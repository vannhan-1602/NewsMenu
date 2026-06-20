namespace Domain.Entities
{
    // 2 cấp: Tỉnh/TP (ParentId = 0) và Phường/Xã (ParentId = ward_id của Tỉnh/TP cha)
    // Đúng theo cải cách hành chính VN hiện hành, không có cấp Quận/Huyện trung gian
    public class Ward : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public int ParentId { get; set; }             // 0 = gốc (Tỉnh/TP), khác 0 = trỏ ward_id của Tỉnh/TP cha
        public Ward? Parent { get; set; }
        public ICollection<Ward> Children { get; set; } = new List<Ward>();

        public int CountryId { get; set; }            // Mọi Ward đều biết quốc gia của mình
        public Country Country { get; set; } = null!;
    }
}
