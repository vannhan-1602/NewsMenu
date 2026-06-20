using Application.Request.News;
using FluentValidation;

namespace Application.Validator.News
{
    public class CreateNewsListValidator : AbstractValidator<CreateNewsListRequest>
    {
        public CreateNewsListValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Items khong duoc rong");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Title)
                    .NotEmpty().WithMessage("Title khong duoc de trong")
                    .MaximumLength(500).WithMessage("Title khong vuot qua 500 ky tu");

                item.RuleFor(i => i.Content)
                    .NotEmpty().WithMessage("Content khong duoc de trong");

                item.RuleFor(i => i.Summary)
                    .MaximumLength(1000).WithMessage("Summary khong vuot qua 1000 ky tu")
                    .When(i => i.Summary != null);
            });
        }
    }
}
