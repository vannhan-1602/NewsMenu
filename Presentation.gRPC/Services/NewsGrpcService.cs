using Application.DTOs;
using Application.Features.Menus.Commands.CreateMenu;
using Application.Features.Menus.Commands.DeleteMenu;
using Application.Features.Menus.Commands.UpdateMenu;
using Application.Features.Menus.Queries.GetMenuById;
using Application.Features.Menus.Queries.GetMenuList;
using Application.Features.News.Commands.CreateNews;
using Application.Features.News.Commands.DeleteNews;
using Application.Features.News.Commands.UpdateNews;
using Application.Features.News.Queries.GetNewsById;
using Application.Features.News.Queries.GetNewsList;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using NewsMenu.Presentation.gRPC.Protos;
namespace Presentation.gRPC.Services
{
    public class NewsGrpcService : NewsService.NewsServiceBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NewsGrpcService> _logger;

        public NewsGrpcService(IMediator mediator, ILogger<NewsGrpcService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        public override async Task<CreateNewsResponse> CreateNews(
            CreateNewsRequest request,
            ServerCallContext context)
        {
            _logger.LogInformation("gRPC CreateNews: '{Title}'", request.Title);

            try
            {
                var command = new CreateNewsCommand(
                    Title: request.Title,
                    Content: request.Content,
                    Summary: string.IsNullOrEmpty(request.Summary) ? null : request.Summary,
                    IsPublished: request.IsPublished,
                    MenuIds: request.MenuIds.Select(Guid.Parse).ToList()
                );

               
                var newsId = await _mediator.Send(command, context.CancellationToken);

                return new CreateNewsResponse
                {
                    NewsId = newsId.ToString(),
                    Message = "Tạo News thành công"
                };
            }
            catch (ValidationException ex)
            {
                
                var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi CreateNews");
                throw new RpcException(new Status(StatusCode.Internal, "Lỗi hệ thống"));
            }
        }

       
        public override async Task<GetNewsByIdResponse> GetNewsById(
            GetNewsByIdRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.NewsId, out var newsId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "NewsId không hợp lệ"));

            var result = await _mediator.Send(
                new GetNewsByIdQuery(newsId),
                context.CancellationToken);

            if (result == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Không tìm thấy News {request.NewsId}"));

            return MapToResponse(result);
        }

    
        public override async Task<GetNewsListResponse> GetNewsList(
            GetNewsListRequest request,
            ServerCallContext context)
        {
            var query = new GetNewsListQuery(
                Page: request.Page > 0 ? request.Page : 1,
                PageSize: request.PageSize > 0 ? request.PageSize : 10);

            var results = await _mediator.Send(query, context.CancellationToken);

            var response = new GetNewsListResponse();
            response.Items.AddRange(results.Select(MapToResponse));
            return response;
        }

        private static GetNewsByIdResponse MapToResponse(Application.DTOs.NewsDto dto)
        {
            var response = new GetNewsByIdResponse
            {
                NewsId = dto.Id.ToString(),
                Title = dto.Title,
                Content = dto.Content,
                Summary = dto.Summary ?? string.Empty,
                IsPublished = dto.IsPublished,
                CreatedAt = dto.CreatedAt.ToString("o"),
            };

            response.Menus.AddRange(dto.Menus.Select(m => new MenuResponse
            {
                MenuId = m.Id.ToString(),
                Name = m.Name,
                Slug = m.Slug,
                DisplayOrder = m.DisplayOrder,
            }));

            return response;
        }
        public override async Task<BaseResponse> UpdateNews(UpdateNewsRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.NewsId, out var newsId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "NewsId không hợp lệ"));

            var command = new UpdateNewsCommand(
                newsId,
                request.Title,
                request.Content,
                string.IsNullOrEmpty(request.Summary) ? null : request.Summary,
                request.IsPublished,
                request.MenuIds.Select(Guid.Parse).ToList()
            );

            await _mediator.Send(command, context.CancellationToken);

            return new BaseResponse
            {
                Success = true,
                Message = "Cập nhật News thành công",
                Id = request.NewsId
            };
        }

        public override async Task<BaseResponse> DeleteNews(DeleteRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var newsId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "NewsId không hợp lệ"));

            await _mediator.Send(new DeleteNewsCommand(newsId), context.CancellationToken);

            return new BaseResponse
            {
                Success = true,
                Message = "Đã xóa mềm News thành công",
                Id = request.Id
            };
        }
        public override async Task<BaseResponse> CreateMenu(CreateMenuRequest request, ServerCallContext context)
        {
            var command = new CreateMenuCommand(request.Name, request.Slug, request.DisplayOrder);
            var menuId = await _mediator.Send(command, context.CancellationToken);

            return new BaseResponse
            {
                Success = true,
                Message = "Tạo Menu thành công",
                Id = menuId.ToString()
            };
        }

        public override async Task<BaseResponse> UpdateMenu(UpdateMenuRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.MenuId, out var menuId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "MenuId không hợp lệ"));

            var command = new UpdateMenuCommand(menuId, request.Name, request.Slug, request.DisplayOrder);
            await _mediator.Send(command, context.CancellationToken);

            return new BaseResponse
            {
                Success = true,
                Message = "Cập nhật Menu thành công",
                Id = request.MenuId
            };
        }

        public override async Task<BaseResponse> DeleteMenu(DeleteRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var menuId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "MenuId không hợp lệ"));

            await _mediator.Send(new DeleteMenuCommand(menuId), context.CancellationToken);

            return new BaseResponse
            {
                Success = true,
                Message = "Đã xóa Menu thành công",
                Id = request.Id
            };
        }

        public override async Task<MenuResponse> GetMenuById(GetByIdRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var menuId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "MenuId không hợp lệ"));

            var result = await _mediator.Send(new GetMenuByIdQuery(menuId), context.CancellationToken);
            if (result == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Không tìm thấy Menu {request.Id}"));

            return new MenuResponse
            {
                MenuId = result.Id.ToString(),
                Name = result.Name,
                Slug = result.Slug,
                DisplayOrder = result.DisplayOrder
            };
        }

        public override async Task<GetMenuListResponse> GetMenuList(GetListRequest request, ServerCallContext context)
        {
            var results = await _mediator.Send(new GetMenuListQuery(), context.CancellationToken);
            var response = new GetMenuListResponse();

            response.Items.AddRange(results.Select(m => new MenuResponse
            {
                MenuId = m.Id.ToString(),
                Name = m.Name,
                Slug = m.Slug,
                DisplayOrder = m.DisplayOrder
            }));

            return response;
        }
    }
}
