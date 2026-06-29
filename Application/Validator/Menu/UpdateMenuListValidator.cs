using Application.Request.Menu;
using FluentValidation;

namespace Application.Validator.Menu
{
    public class UpdateMenuListValidator : AbstractValidator<UpdateMenuListRequest>
    {
        public UpdateMenuListValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Items không được rỗng");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(menuItem => menuItem.Id)
                    .GreaterThan(0).WithMessage("Id không hợp lệ");

                item.RuleFor(menuItem => menuItem.Name)
                    .NotEmpty().WithMessage("Name không được để trống")
                    .MaximumLength(255).WithMessage("Name không vượt quá 255 ký tự");

                item.RuleFor(menuItem => menuItem.Slug)
                    .NotEmpty().WithMessage("Slug không được để trống")
                    .MaximumLength(255).WithMessage("Slug không vượt quá 255 ký tự")
                    .Matches("^[a-z0-9-]+$").WithMessage("Slug chỉ được chứa chữ thường, số và dấu gạch ngang");

                item.RuleFor(menuItem => menuItem.DisplayOrder)
                    .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder phải >= 0");
            });
        }
    }
}
