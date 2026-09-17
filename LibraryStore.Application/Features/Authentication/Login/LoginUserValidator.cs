using FluentValidation;

namespace LibraryStore.Application.Features.Authentication.Login
{
    public class LoginUserValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("لا يمكن ترك البريد الالكتروني فارغ ")
                .MaximumLength(400).WithMessage("يجب الا يتعدي البريد الالكتروني 400 حرف");
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("لا يمكن ترك كلمة المرور فارغه")
                .MinimumLength(8).WithMessage("كلمة المرور يجب ان تكون اكثر من 8 حروف ");
        }
    }
}
