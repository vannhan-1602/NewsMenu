using Application.DTOs;
using Application.Request.Address;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Address
{
    public class GetCountryListUseCase : IRequestHandler<GetCountryListRequest, List<CountryDto>>
    {
        private readonly ICountryRepository _countryRepository;

        public GetCountryListUseCase(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<CountryDto>> Handle(GetCountryListRequest request, CancellationToken ct)
        {
            //chỉ lấy Id + Name cho dropdown, không tải dư field
            var query = _countryRepository.Query()
                .OrderBy(c => c.Name)
                .Select(c => new CountryDto
                {
                    Id = c.Id,
                    Name = c.Name
                });

            var result = new List<CountryDto>();
            await foreach (var item in query.AsAsyncEnumerable().WithCancellation(ct))
                result.Add(item);

            return result;
        }
    }
}
