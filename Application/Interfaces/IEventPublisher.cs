using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(string routingKey, T @event, CancellationToken ct = default);
    }
}
