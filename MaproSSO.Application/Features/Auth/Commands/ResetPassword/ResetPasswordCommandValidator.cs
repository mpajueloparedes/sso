using FluentValidation;

namespace MaproSSO.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("El formato del email no es válido");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("El token es requerido");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La nueva contraseña es requerida")
                .MinimumLength(12).WithMessage("La contraseña debe tener al menos 12 caracteres")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número")
                .Matches(@"[!@#$%^&*(),.?"":{}|<>]").WithMessage("La contraseña debe contener al menos un carácter especial");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword).WithMessage("Las contraseñas no coinciden");
        }
    }
}