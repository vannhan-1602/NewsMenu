using Application.Common;
using Application.DTOs;
using Application.Request.News;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> Create([FromBody] CreateNewsRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<BaseResponse>> CreateMany([FromBody] CreateNewsListRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return Ok(result);
        }

        [HttpPut("batch")]
        public async Task<ActionResult<BaseResponse>> UpdateMany([FromBody] UpdateNewsListRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IAsyncEnumerable<NewsDto>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            return await _mediator.Send(new GetNewsListRequest { Page = page, PageSize = pageSize }, ct);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NewsDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetNewsByIdRequest(id), ct);
            if (result == null)
            {
                return NotFound(new BaseResponse { Success = false, Message = $"Không tìm thấy News {id}" });
            }
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseResponse>> Update(int id, [FromBody] UpdateNewsRequest request, CancellationToken ct)
        {
            request.Id = id;
            var result = await _mediator.Send(request, ct);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<BaseResponse>> Delete(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteNewsRequest(id), ct);
            return Ok(result);
        }

        [HttpDelete("batch")]
        public async Task<ActionResult<BaseResponse>> DeleteMany([FromBody] DeleteNewsListRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return Ok(result);
        }
    }
}
