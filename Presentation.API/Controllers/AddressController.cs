using Application.DTOs;
using Application.Request.Address;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /api/address/countries
        [HttpGet("countries")]
        public async Task<IAsyncEnumerable<CountryDto>> GetCountries(CancellationToken ct)
        {
            return await _mediator.Send(new GetCountryListRequest(), ct);
        }

        // GET /api/address/wards?parentId=0 → Tỉnh/TP
        // GET /api/address/wards?parentId=2 → Phường/Xã thuộc TP HCM
        [HttpGet("wards")]
        public async Task<IAsyncEnumerable<WardDto>> GetWards(
            [FromQuery] int parentId = 0,
            CancellationToken ct = default)
        {
            return await _mediator.Send(new GetWardListRequest { ParentId = parentId }, ct);
        }
    }
}
