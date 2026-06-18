using Application.Request;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Validator
{
    public class UpdateNewsValidator : AbstractValidator<UpdateNewsRequest>
    {
        public UpdateNewsValidator(INewsRepository newsRepository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("NewsId khong duoc de trong")
                .MustAsync(async (id, ct) => await newsRepository.GetByIdAsync(id, ct) != null)
                .WithMessage("News khong ton tai hoac da bi xoa");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title khong duoc de trong")
                .MaximumLength(500).WithMessage("Title khong duoc vuot qua 500 ky tu");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content khong duoc de trong");

            RuleFor(x => x.Summary)
                .MaximumLength(1000).WithMessage("Summary khong duoc vuot qua 1000 ky tu")
                .When(x => x.Summary != null);

            RuleFor(x => x.MenuIds)
                .NotNull().WithMessage("MenuIds khong duoc null");
        }
    }
}
