using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.Auth.Commands.EnableTwoFactor
{
    [Authorize]
    public class EnableTwoFactorCommand : IRequest<Result<EnableTwoFactorResponse>>
    {
        public string Method { get; set; } // SMS, Email, Authenticator
        public string VerificationCode { get; set; }
    }
}