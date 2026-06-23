using Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.API.Filters
{
    
    public class ValidationExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException ex)
            {
                context.Result = new BadRequestObjectResult(new BaseResponse
                {
                    Success = false,
                    // Nối tất cả lỗi thành một chuỗi để trả về client
                    Message = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))
                });

                // Đánh dấu exception đã được xử lý
                context.ExceptionHandled = true;
            }
        }
    }
}