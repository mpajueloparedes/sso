using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.Auth.Commands.Logout
{
    [Authorize]
    public class LogoutCommand : IRequest<Result>
    {
        public string RefreshToken { get; set; }
        public string SessionId { get; set; }
        public bool LogoutFromAllDevices { get; set; }
    }
}