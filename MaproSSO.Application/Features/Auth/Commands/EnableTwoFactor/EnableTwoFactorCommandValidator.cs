using FluentValidation;

namespace MaproSSO.Application.Features.Auth.Commands.EnableTwoFactor
{
    public class EnableTwoFactorCommandValidator : AbstractValidator<EnableTwoFactorCommand>
    {
        public EnableTwoFactorCommandValidator()
        {
            RuleFor(x => x.Method)
                .NotEmpty().WithMessage("El método es requerido") // SMS, Email, Authenticator
                .MaximumLength(50).WithMessage("El método no puede exceder los 50 caracteres");

            RuleFor(x => x.VerificationCode)
                .NotEmpty().WithMessage("El código de verificación es requerido")
                .Matches(@"^\d{6}$").WithMessage("El código debe tener 6 dígitos numéricos"); // ejemplo para códigos tipo OTP
        }
    }
}