using FluentValidation;
using ITDestek.Models.Entities;

namespace ITDestek.Validators;

public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
{
    public ApplicationUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi gereklidir.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .Must(BeValidTuzlaDomain).WithMessage("Sadece @tuzla.bel.tr uzantılı kurumsal e-posta adresleri kullanılabilir.");
            
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad gereklidir.")
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad gereklidir.")
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");
    }
    
    private bool BeValidTuzlaDomain(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
            
        return email.EndsWith("@tuzla.bel.tr", StringComparison.OrdinalIgnoreCase);
    }
}
