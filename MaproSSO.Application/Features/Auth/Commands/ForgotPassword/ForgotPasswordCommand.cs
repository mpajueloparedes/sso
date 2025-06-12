using MediatR;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }
}