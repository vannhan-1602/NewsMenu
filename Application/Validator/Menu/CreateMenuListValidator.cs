using Application.Request.Menu;
using FluentValidation;

namespace Application.Validator.Menu
{
    public class CreateMenuListValidator : AbstractValidator<CreateMenuListRequest>
    {
        public CreateMenuListValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Items khong duoc rong");

         
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Name)
                    .NotEmpty().WithMessage("Name khong duoc de trong")
                    .MaximumLength(255).WithMessage("Name khong vuot qua 255 ky tu");

                item.RuleFor(i => i.Slug)
                    .NotEmpty().WithMessage("Slug khong duoc de trong")
                    .MaximumLength(255).WithMessage("Slug khong vuot qua 255 ky tu")
                    .Matches("^[a-z0-9-]+$").WithMessage("Slug chi duoc chua chu thuong, so va dau gach ngang");

                item.RuleFor(i => i.DisplayOrder)
                    .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder phai >= 0");
            });
        }
    }
}
