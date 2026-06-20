using Application.Request.Menu;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Validator.Menu
{
    public class UpdateMenuValidator : AbstractValidator<UpdateMenuRequest>
    {
        public UpdateMenuValidator(IMenuRepository menuRepository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("MenuId khong duoc de trong")
                .MustAsync(async (id, ct) => await menuRepository.GetByIdAsync(id, ct) != null)
                .WithMessage("Menu khong ton tai hoac da bi xoa");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name khong duoc de trong")
                .MaximumLength(255).WithMessage("Name khong duoc vuot qua 255 ky tu");

            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug khong duoc de trong")
                .MaximumLength(255).WithMessage("Slug khong duoc vuot qua 255 ky tu")
                .Matches("^[a-z0-9-]+$").WithMessage("Slug chi duoc chua chu thuong, so va dau gach ngang")
                .MustAsync(async (request, slug, ct) =>
                {
                    var existing = await menuRepository.GetBySlugAsync(slug, ct);
                    return existing == null || existing.Id == request.Id;
                })
                .WithMessage("Slug da duoc su dung boi menu khac");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder phai >= 0");

            RuleFor(x => x.NewsIds)
                .NotNull().WithMessage("NewsIds khomg duoc null");
        }
    }
}
