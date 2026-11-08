using FluentValidation;

namespace LibraryStore.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderValidatioin : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidatioin()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .WithMessage("اسم العميل مطلوب.")
                .MaximumLength(100)
                .WithMessage("اسم العميل يجب ألا يتجاوز 100 حرف.");

            RuleFor(x => x.CustomerPhone)
                .NotEmpty()
                .WithMessage("رقم هاتف العميل مطلوب.")
                .Matches(@"^\d{11}$")
                .WithMessage("رقم الهاتف يجب أن يتكون من 11 رقمًا.");

            RuleFor(x => x.CustomerAddress)
                .NotEmpty()
                .WithMessage("عنوان العميل مطلوب.")
                .MaximumLength(400)
                .WithMessage("العنوان يجب ألا يتجاوز 400 حرفًا.");

            RuleFor(x => x.Items)
                .NotNull()
                .WithMessage("عناصر الطلب مطلوبة.")
                .NotEmpty()
                .WithMessage("يجب أن يحتوي الطلب على منتج واحد على الأقل.");

            RuleFor(x => x.Items)
                .Must(items =>
                    items.Select(item => item.ProductId).Distinct().Count() == items.Count)
                .When(x => x.Items is not null)
                .WithMessage("لا يمكن إضافة المنتج نفسه أكثر من مرة.");

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.ProductId)
                        .NotEmpty()
                        .WithMessage("معرّف المنتج مطلوب.");

                    item.RuleFor(x => x.Quantity)
                        .GreaterThan(0)
                        .WithMessage("الكمية يجب أن تكون أكبر من صفر.");
                });
        }
    }
}
