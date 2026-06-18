using Application.Common;
using Application.DTOs;
using Application.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> Create([FromBody] CreateNewsRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<List<NewsDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetNewsListRequest { Page = page, PageSize = pageSize }, ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NewsDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetNewsByIdRequest(id), ct);
            return result == null
                ? NotFound(new BaseResponse { Success = false, Message = $"Không tìm thấy News {id}" })
                : Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BaseResponse>> Update(int id, [FromBody] UpdateNewsRequest request, CancellationToken ct)
        {
            request.Id = id;
            return Ok(await _mediator.Send(request, ct));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<BaseResponse>> Delete(int id, CancellationToken ct)
            => Ok(await _mediator.Send(new DeleteNewsRequest(id), ct));
    }
}