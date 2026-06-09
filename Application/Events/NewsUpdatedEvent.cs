using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Events
{
    public record NewsUpdatedEvent(Guid NewsId, string Title, string Content, string? Summary, bool IsPublished, List<Guid> MenuIds);
}