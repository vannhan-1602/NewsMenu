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
        public async Task<ActionResult<List<CountryDto>>> GetCountries(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCountryListRequest(), ct);
            return Ok(result);
        }

        // GET /api/address/wards?parentId=0   → Tỉnh/TP
        // GET /api/address/wards?parentId= khác 0   → Phường/Xã 
      
        [HttpGet("wards")]
        public async Task<ActionResult<List<WardDto>>> GetWards(
            [FromQuery] int parentId = 0,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetWardListRequest { ParentId = parentId }, ct);
            return Ok(result);
        }
    }
}
