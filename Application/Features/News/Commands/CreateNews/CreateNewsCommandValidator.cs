using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.News.Commands.CreateNews
{
    public class CreateNewsCommandValidator : AbstractValidator<CreateNewsCommand>
    {
        public CreateNewsCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề không được để trống")
                .MaximumLength(500).WithMessage("Tiêu đề tối đa 500 ký tự");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung không được để trống");

            RuleFor(x => x.Summary)
                .MaximumLength(1000).When(x => x.Summary != null)
                .WithMessage("Tóm tắt tối đa 1000 ký tự");

            RuleFor(x => x.MenuIds)
                .NotNull().WithMessage("Danh sách menu không được null")
                .Must(ids => ids.Count > 0).WithMessage("Phải chọn ít nhất 1 menu");
        }
    }
}
