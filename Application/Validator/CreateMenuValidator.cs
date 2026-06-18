using Application.Request;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Validator
{
  
    public class CreateMenuValidator : AbstractValidator<CreateMenuRequest>
    {
        public CreateMenuValidator(IMenuRepository menuRepository)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name khong duoc de trong")
                .MaximumLength(255).WithMessage("Name khong vuot qua 255 ky tu");

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug khong duoc de trong")
                .MaximumLength(255).WithMessage("Slug khong vuot qua 255 ky tu")
                .Matches("^[a-z0-9-]+$").WithMessage("Slug chi duoc chua chu thuong, so va dau gach ngang")
                .MustAsync(async (slug, ct) => await menuRepository.GetBySlugAsync(slug, ct) == null)
                .WithMessage("Slug da ton tai");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder phai >= 0");

            RuleFor(x => x.NewsIds)
                .NotNull().WithMessage("NewsIds khong duoc null");
        }
    }
}
