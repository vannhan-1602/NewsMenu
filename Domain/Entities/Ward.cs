namespace Domain.Entities
{
   
    public class Ward : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public int ParentId { get; set; }            
        public Ward? Parent { get; set; }
        public ICollection<Ward> Children { get; set; } = new List<Ward>();

        public int CountryId { get; set; }            
        public Country Country { get; set; } = null!;
    }
}
