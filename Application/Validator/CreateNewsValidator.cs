using Application.Request;
using FluentValidation;

namespace Application.Validator
{
    
    public class CreateNewsValidator : AbstractValidator<CreateNewsRequest>
    {
        public CreateNewsValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title khong duoc de trong")
                .MaximumLength(500).WithMessage("Title khong duoc vuot qua 500 ky tu");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content không được để trống");

            RuleFor(x => x.Summary)
                .MaximumLength(1000).WithMessage("Summary khong duoc vuot qua 1000 ky tu")
                .When(x => x.Summary != null);

            RuleFor(x => x.MenuIds)
                .NotNull().WithMessage("MenuIds khong duoc null");
        }
    }
}
