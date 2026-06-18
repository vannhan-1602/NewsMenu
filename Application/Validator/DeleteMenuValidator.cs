using Application.Request;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Validator
{
    public class DeleteMenuValidator : AbstractValidator<DeleteMenuRequest>
    {
        public DeleteMenuValidator(IMenuRepository menuRepository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("MenuId khong duoc de trong")
                .MustAsync(async (id, ct) => await menuRepository.GetByIdAsync(id, ct) != null)
                .WithMessage("Menu khong ton tai hoac da bi xoa");
        }
    }
}
