using Application.DTOs;
using MediatR;

namespace Application.Request.Address
{
    // parentId = 0  trả Tỉnh/TP
    // parentId = ward_id trả Phường/Xã thuộc Tỉnh/TP đó
    public class GetWardListRequest : IRequest<List<WardDto>>
    {
        public int ParentId { get; set; } = 0;
    }
}
