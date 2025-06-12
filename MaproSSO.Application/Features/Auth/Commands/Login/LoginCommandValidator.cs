using FluentValidation;

namespace MaproSSO.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El usuario es requerido")
                .MaximumLength(100).WithMessage("El usuario no puede exceder 100 caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida");

            RuleFor(x => x.IpAddress)
                .NotEmpty().WithMessage("La dirección IP es requerida")
                .Matches(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$|^(?:[a-fA-F0-9]{1,4}:){7}[a-fA-F0-9]{1,4}$")
                .WithMessage("Formato de IP inválido");
        }
    }
}