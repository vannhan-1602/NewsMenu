using Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.API.Filters
{
    // Filter chạy SAU khi action throw exception, TRƯỚC khi trả response cho client.
    // Thay thế try/catch 
    public class ValidationExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException ex)
            {
                context.Result = new BadRequestObjectResult(new BaseResponse
                {
                    Success = false,
                    Message = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))
                });

                // Đánh dấu exception đã được xử lý
                context.ExceptionHandled = true;
            }
        }
    }
}