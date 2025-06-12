namespace MaproSSO.Application.Common.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string password);
        bool ValidatePasswordStrength(string password, out string[] errors);
    }
}