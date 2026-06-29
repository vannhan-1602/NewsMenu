using Application.Request.News;
using FluentValidation;

namespace Application.Validator.News
{
    public class UpdateNewsListValidator : AbstractValidator<UpdateNewsListRequest>
    {
        public UpdateNewsListValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Items không được rỗng");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(newsItem => newsItem.Id)
                    .GreaterThan(0).WithMessage("Id không hợp lệ");

                item.RuleFor(newsItem => newsItem.Title)
                    .NotEmpty().WithMessage("Title không được để trống")
                    .MaximumLength(500).WithMessage("Title không vượt quá 500 ký tự");

                item.RuleFor(newsItem => newsItem.Content)
                    .NotEmpty().WithMessage("Content không được để trống");

                item.RuleFor(newsItem => newsItem.Summary)
                    .MaximumLength(1000).WithMessage("Summary không vượt quá 1000 ký tự")
                    .When(newsItem => newsItem.Summary != null);
            });
        }
    }
}
