using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
namespace Application.Features.News.Commands.DeleteNews
{
    public record DeleteNewsCommand(Guid NewsId) : IRequest<bool>;
}
