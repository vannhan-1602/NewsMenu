using Application.DTOs;
using MediatR;

namespace Application.Request.Address
{
    public class GetCountryListRequest : IRequest<IAsyncEnumerable<CountryDto>>
    {
    }
}
