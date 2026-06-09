using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Features.News.Commands.CreateNews
{
    public record CreateNewsCommand(
        string Title,
        string Content,
        string? Summary,
        bool IsPublished,
        List<Guid> MenuIds
    ) : IRequest<Guid>;
}
