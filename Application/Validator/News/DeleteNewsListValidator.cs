using Application.Request.News;
using FluentValidation;

namespace Application.Validator.News
{
    public class DeleteNewsListValidator : AbstractValidator<DeleteNewsListRequest>
    {
        public DeleteNewsListValidator()
        {
            RuleFor(x => x.Ids)
                .NotEmpty().WithMessage("Ids khong duoc rong");

            RuleForEach(x => x.Ids)
                .GreaterThan(0).WithMessage("Id khong hop le");
        }
    }
}
