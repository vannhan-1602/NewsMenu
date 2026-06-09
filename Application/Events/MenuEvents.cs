using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Events
{
    public record MenuCreatedEvent(Guid MenuId, string Name, string Slug, int DisplayOrder);
    public record MenuUpdatedEvent(Guid MenuId, string Name, string Slug, int DisplayOrder);
    public record MenuDeletedEvent(Guid MenuId);
}
