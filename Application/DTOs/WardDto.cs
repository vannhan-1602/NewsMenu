namespace Application.DTOs
{
    
    public class WardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ParentId { get; set; }   // 0 = Tỉnh/TP, khác 0 = Phường/Xã
    }
}
