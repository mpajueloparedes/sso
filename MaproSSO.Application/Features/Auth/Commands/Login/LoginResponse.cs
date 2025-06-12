using System;
using System.Collections.Generic;

namespace MaproSSO.Application.Features.Auth.Commands.Login
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string SessionId { get; set; }
        public UserDto User { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool MustChangePassword { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PhotoUrl { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string TimeZone { get; set; }
        public string Culture { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Permissions { get; set; }
    }
}