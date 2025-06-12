using MediatR;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponse>>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IpAddress { get; set; }
    }
}