using Application.DTOs;
using Application.Request.Address;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Address
{
    public class GetWardListUseCase : IRequestHandler<GetWardListRequest, IAsyncEnumerable<WardDto>>
    {
        private readonly IWardRepository _wardRepository;

        public GetWardListUseCase(IWardRepository wardRepository)
        {
            _wardRepository = wardRepository;
        }

        public Task<IAsyncEnumerable<WardDto>> Handle(GetWardListRequest request, CancellationToken ct)
        {
            var result = _wardRepository.Query()
                .Where(w => w.ParentId == request.ParentId)
                .OrderBy(w => w.Name)
                .Select(w => new WardDto { Id = w.Id, Name = w.Name, ParentId = w.ParentId })
                .AsAsyncEnumerable();

            return Task.FromResult(result);
        }
    }
}
