using FluentValidation;

namespace MaproSSO.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("La contraseña actual es requerida");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La nueva contraseña es requerida")
                .MinimumLength(12).WithMessage("La contraseña debe tener al menos 12 caracteres")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
                .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
                .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número")
                .Matches(@"[!@#$%^&*(),.?"":{}|<>]").WithMessage("La contraseña debe contener al menos un carácter especial");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Las contraseñas no coinciden");
        }
    }
}