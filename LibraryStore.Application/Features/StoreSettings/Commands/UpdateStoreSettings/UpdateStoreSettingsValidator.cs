using FluentValidation;

namespace LibraryStore.Application.Features.StoreSettings.Commands.UpdateStoreSettings
{
    public class UpdateStoreSettingsValidator : AbstractValidator<UpdateStoreSettingsCommand>
    {
        public UpdateStoreSettingsValidator()
        {
            RuleFor(x => x.StoreName)
                .NotEmpty().WithMessage("اسم المتجر مطلوب")
                .MaximumLength(100).WithMessage("اسم المتجر يجب ألا يتجاوز 100 حرف");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("رقم الهاتف مطلوب")
                .MaximumLength(20).WithMessage("رقم الهاتف يجب ألا يتجاوز 20 رقم")
                .Matches(@"^[0-9+\-\s]+$").WithMessage("رقم الهاتف غير صالح");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("العنوان مطلوب")
                .MaximumLength(200).WithMessage("العنوان يجب ألا يتجاوز 200 حرف");

            RuleFor(x => x.Facebook)
                .MaximumLength(500).WithMessage("الرابط يجب ألا يتجاوز 500 حرف")
                .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.Facebook))
                .WithMessage("رابط Facebook غير صالح");

            RuleFor(x => x.Instagram)
                .MaximumLength(500).WithMessage("الرابط يجب ألا يتجاوز 500 حرف")
                .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.Instagram))
                .WithMessage("رابط Instagram غير صالح");

            RuleFor(x => x.WhatsApp)
                .MaximumLength(500).WithMessage("الرابط يجب ألا يتجاوز 500 حرف")
                .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.WhatsApp))
                .WithMessage("رابط WhatsApp غير صالح");

            RuleFor(x => x.TikTok)
                .MaximumLength(500).WithMessage("الرابط يجب ألا يتجاوز 500 حرف")
                .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.TikTok))
                .WithMessage("رابط TikTok غير صالح");
        }

        private static bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}