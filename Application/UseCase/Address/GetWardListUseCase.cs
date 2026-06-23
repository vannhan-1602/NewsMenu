using Application.DTOs;
using Application.Request.Address;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Address
{
    public class GetWardListUseCase : IRequestHandler<GetWardListRequest, List<WardDto>>
    {
        private readonly IWardRepository _wardRepository;

        public GetWardListUseCase(IWardRepository wardRepository)
        {
            _wardRepository = wardRepository;
        }

        public async Task<List<WardDto>> Handle(GetWardListRequest request, CancellationToken ct)
        {
            // parentId = 0 = lấy Tỉnh/TP
            // parentId != 0 = lấy Phường/Xã thuộc Tỉnh/TP đó
            var query = _wardRepository.Query()
                .Where(w => w.ParentId == request.ParentId)
                .OrderBy(w => w.Name)
                .Select(w => new WardDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    ParentId = w.ParentId
                });

            var result = new List<WardDto>();
            await foreach (var item in query.AsAsyncEnumerable().WithCancellation(ct))
                result.Add(item);

            return result;
        }
    }
}
