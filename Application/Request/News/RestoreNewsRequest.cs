using Application.Common;
using MediatR;

public class RestoreNewsRequest : IRequest<BaseResponse>
{
    public int Id { get; set; }
}