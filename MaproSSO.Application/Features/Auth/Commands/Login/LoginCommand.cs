using MediatR;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<LoginResponse>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public string DeviceInfo { get; set; }
        public bool RememberMe { get; set; }
    }
}