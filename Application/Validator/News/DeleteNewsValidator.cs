using Application.Request.News;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Validator.News
{
    public class DeleteNewsValidator : AbstractValidator<DeleteNewsRequest>
    {
        public DeleteNewsValidator(INewsRepository newsRepository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("NewsId khong duoc de trong")
                .MustAsync(async (id, ct) => await newsRepository.GetByIdAsync(id, ct) != null)
                .WithMessage("News khong ton tai hoac da bi xoa");
        }
    }
}
