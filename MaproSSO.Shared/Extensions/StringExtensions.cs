using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MaproSSO.Shared.Extensions;

public static class StringExtensions
{
    public static string ToSlug(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Remove diacritics
        text = text.RemoveDiacritics();

        // Replace spaces and invalid characters with hyphens
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", "-");
        text = Regex.Replace(text, @"-+", "-");

        // Trim hyphens from start and end
        return text.Trim('-');
    }

    public static string RemoveDiacritics(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var normalizedText = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedText)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string Truncate(this string text, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;

        return text[..(maxLength - suffix.Length)] + suffix;
    }

    public static string ToTitleCase(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }

    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    public static string MaskEmail(this string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            return email;

        var parts = email.Split('@');
        var localPart = parts[0];
        var domainPart = parts[1];

        if (localPart.Length <= 2)
            return email;

        var maskedLocal = localPart[0] + new string('*', localPart.Length - 2) + localPart[^1];
        return $"{maskedLocal}@{domainPart}";
    }

    public static string FormatPhoneNumber(this string phoneNumber, string format = "+{0} {1} {2}-{3}")
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return string.Empty;

        var digitsOnly = Regex.Replace(phoneNumber, @"[^\d]", "");

        return digitsOnly.Length switch
        {
            11 when digitsOnly.StartsWith("1") => string.Format(format, digitsOnly[..1], digitsOnly[1..4], digitsOnly[4..7], digitsOnly[7..]),
            10 => string.Format(format, "1", digitsOnly[..3], digitsOnly[3..6], digitsOnly[6..]),
            _ => phoneNumber
        };
    }
}