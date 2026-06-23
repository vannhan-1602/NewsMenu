namespace Domain.Entities
{
   
    public class MenuNews
    {
        public int MenuId { get; set; }
        public int NewsId { get; set; }
        public DateTime AssignedAt { get; set; }

       
        public Menu Menu { get; set; } = null!;
        public News News { get; set; } = null!;
    }
}
