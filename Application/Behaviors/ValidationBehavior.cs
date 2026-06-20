using FluentValidation;
using MediatR;

namespace Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            //nếu không có validator nào thì bỏ qua và chạy usecase
            if (!_validators.Any()) return await next(ct);

            //tạo context chứa object request để validator có thể lấy ra thông tin 
            var context = new ValidationContext<TRequest>(request);

            // ValidateAsync để MustAsync chạy đúng
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct)));

            //gom nhóm lỗi lại thành một list để trả về cho client
            var failures = results.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);

            return await next(ct);
        }
    }
}
