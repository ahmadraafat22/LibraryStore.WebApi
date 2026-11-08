using FluentValidation;
using LibraryStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LibraryStore.Application.Features.Authentication.Register
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public RegisterUserValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("يجب ادخال  بريد الكتروني ")
                .EmailAddress()
                .MustAsync(async (email, cancellationToken) =>
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    return user == null;

                }
                ).WithMessage("البريد الالكتروني مستخدم بالفعل ");

            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("يجب ادخال اسم المستخدم")
                .MaximumLength(100).WithMessage("يجب الا يتعدي الاسم 100 حرف");

            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("يجب ادخال اسم المستخدم")
                .MaximumLength(100).WithMessage("يجب الا يتعدي الاسم 100 حرف");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("لا يمكن ترك كلمة المرور فارغة ")
                .MinimumLength(8).WithMessage("يجب الا  تقل كلمة المرور عن 8 حروف ");

            RuleFor(u => u.ConfirmPassword)
                .NotEmpty().WithMessage("لا يمكن ترك كلمة المرور فارغة ")
                .Equal(u => u.Password).WithMessage("كلمات المرور غير متطابقة");
        }
    }
}
