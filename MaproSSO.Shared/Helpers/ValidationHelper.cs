//using MaproSSO.Shared.Constants;
//using System.Text.RegularExpressions;

//namespace MaproSSO.Shared.Helpers;

//public static class ValidationHelper
//{
//    public static bool IsValidEmail(string email)
//    {
//        if (string.IsNullOrWhiteSpace(email))
//            return false;

//        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
//        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
//    }

//    public static bool IsValidPassword(string password)
//    {
//        if (string.IsNullOrEmpty(password))
//            return false;

//        if (password.Length < SystemConstants.ValidationRules.MIN_PASSWORD_LENGTH ||
//            password.Length > SystemConstants.ValidationRules.MAX_PASSWORD_LENGTH)
//            return false;

//        // At least one uppercase letter
//        if (!Regex.IsMatch(password, @"[A-Z]"))
//            return false;

//        // At least one lowercase letter
//        if (!Regex.IsMatch(password, @"[a-z]"))
//            return false;

//        // At least one digit
//        if (!Regex.IsMatch(password, @"\d"))
//            return false;

//        // At least one special character
//        if (!Regex.IsMatch(password, @"[^\da-zA-Z]"))
//            return false;

//        return true;
//    }

//    public static List<string> Get