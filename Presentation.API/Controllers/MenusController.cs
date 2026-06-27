using Application.Common;
using Application.DTOs;
using Application.Request.Menu;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> Create([FromBody] CreateMenuRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // Thêm nhiều Menu trong 1 request 
        [HttpPost("batch")]
        public async Task<ActionResult<BaseResponse>> CreateMany([FromBody] CreateMenuListRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return Ok(result);
        }

        // Sửa nhiều Menu trong 1 request 
        [HttpPut("batch")]
        public async Task<ActionResult<BaseResponse>> UpdateMany([FromBody] UpdateMenuListRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IAsyncEnumerable<MenuDto>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            return await _mediator.Send(new GetMenuListRequest { Page = page, PageSize = pageSize }, ct);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MenuDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMenuByIdRequest(id), ct);
            if (result == null)
            {
                return NotFound(new BaseResponse { Success = false, Message = $"Không tìm thấy Menu {id}" });
            }
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseResponse>> Update(int id, [FromBody] UpdateMenuRequest request, CancellationToken ct)
        {
            request.Id = id;
            var result = await _mediator.Send(request, ct);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<BaseResponse>> Delete(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteMenuRequest(id), ct);
            return Ok(result);
        }

        // Xóa nhiều Menu trong 1 request
        [HttpDelete("batch")]
        public async Task<ActionResult<BaseResponse>> DeleteMany([FromBody] DeleteMenuListRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return Ok(result);
        }
        // Lấy danh sách Menu đã xóa
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedListAsync([FromQuery] GetDeletedMenuListRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return Ok(result);
        }
        // Khôi phục Menu đã xóa
        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> RestoreAsync(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new RestoreMenuRequest { Id = id }, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
