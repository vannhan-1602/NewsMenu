namespace Domain.Entities
{
    // Cấp 0 - gốc của cây địa chỉ, không đệ quy (Quốc gia)
    public class Country : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        // 1 Country có nhiều Ward (Tỉnh/TP) thuộc về nó
        public ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }
}
