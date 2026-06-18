using Application.Common;
using Application.DTOs;
using Application.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenusController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> Create([FromBody] CreateMenuRequest request, CancellationToken ct)
        {
            
            var result = await _mediator.Send(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<List<MenuDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetMenuListRequest { Page = page, PageSize = pageSize }, ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MenuDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMenuByIdRequest(id), ct);
            return result == null
                ? NotFound(new BaseResponse { Success = false, Message = $"Không tìm thấy Menu {id}" })
                : Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseResponse>> Update(int id, [FromBody] UpdateMenuRequest request, CancellationToken ct)
        {
            request.Id = id;
            return Ok(await _mediator.Send(request, ct));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<BaseResponse>> Delete(int id, CancellationToken ct)
            => Ok(await _mediator.Send(new DeleteMenuRequest(id), ct));
    }
}