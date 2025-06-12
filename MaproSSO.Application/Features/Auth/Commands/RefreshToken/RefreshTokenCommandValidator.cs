using FluentValidation;
namespace MaproSSO.Application.Features.Auth.Commands.RefreshToken;
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("El token de acceso es requerido");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("El refresh token es requerido");

        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("La dirección IP es requerida");
    }
}
