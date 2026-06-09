using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
namespace Application.Features.News.Commands.UpdateNews
{
    public record UpdateNewsCommand(Guid NewsId, string Title, string Content, string? Summary, bool IsPublished, List<Guid> MenuIds) : IRequest<bool>;
}