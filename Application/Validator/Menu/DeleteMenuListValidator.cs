using Application.Request.Menu;
using FluentValidation;

namespace Application.Validator.Menu
{
    public class DeleteMenuListValidator : AbstractValidator<DeleteMenuListRequest>
    {
        public DeleteMenuListValidator()
        {
            RuleFor(x => x.Ids)
                .NotEmpty().WithMessage("Ids khong duoc rong");

            RuleForEach(x => x.Ids)
                .GreaterThan(0).WithMessage("Id khong hop le");
        }
    }
}
