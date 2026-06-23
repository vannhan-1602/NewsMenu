namespace Domain.Entities
{
    
    public class Country : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }
}
