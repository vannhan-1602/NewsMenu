using Application.DTOs;
using Application.Request.Address;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Address
{
    public class GetCountryListUseCase : IRequestHandler<GetCountryListRequest, IAsyncEnumerable<CountryDto>>
    {
        private readonly ICountryRepository _countryRepository;

        public GetCountryListUseCase(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public Task<IAsyncEnumerable<CountryDto>> Handle(GetCountryListRequest request, CancellationToken ct)
        {
            var result = _countryRepository.Query()
                .OrderBy(c => c.Name)
                .Select(c => new CountryDto { Id = c.Id, Name = c.Name })
                .AsAsyncEnumerable();

            return Task.FromResult(result);
        }
    }
}
