using System.Security.Claims;
using System.Threading.Tasks;
using MaproSSO.Domain.Entities.Security;

namespace MaproSSO.Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);
        Task<string> GenerateRefreshToken();
        Task<ClaimsPrincipal> ValidateAccessToken(string token);
        Task<bool> ValidateRefreshToken(string token);
        Task RevokeRefreshToken(string token);
        string GenerateEmailConfirmationToken(string email);
        bool ValidateEmailConfirmationToken(string token, string email);
        string GeneratePasswordResetToken(string email);
        bool ValidatePasswordResetToken(string token, string email);
    }
}