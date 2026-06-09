using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MenuNews
    {
        public Guid MenuId { get; set; }
        public Guid NewsId { get; set; }
        public DateTime AssignedAt { get; set; }

        public Menu Menu { get; set; } = null!;
        public News News { get; set; } = null!;
    }
}
