using System.Globalization;

namespace MaproSSO.Shared.Extensions;

public static class DateTimeExtensions
{
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        return timeSpan.TotalDays switch
        {
            >= 365 => $"hace {(int)(timeSpan.TotalDays / 365)} año(s)",
            >= 30 => $"hace {(int)(timeSpan.TotalDays / 30)} mes(es)",
            >= 7 => $"hace {(int)(timeSpan.TotalDays / 7)} semana(s)",
            >= 1 => $"hace {(int)timeSpan.TotalDays} día(s)",
            _ => timeSpan.TotalHours >= 1 ? $"hace {(int)timeSpan.TotalHours} hora(s)" :
                timeSpan.TotalMinutes >= 1 ? $"hace {(int)timeSpan.TotalMinutes} minuto(s)" : "hace un momento"
        };
    }

    public static string ToDisplayFormat(this DateTime dateTime, string format = "dd/MM/yyyy HH:mm")
    {
        return dateTime.ToString(format, CultureInfo.CurrentCulture);
    }

    public static string ToDisplayDate(this DateTime dateTime)
    {
        return dateTime.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);
    }

    public static string ToDisplayTime(this DateTime dateTime)
    {
        return dateTime.ToString("HH:mm", CultureInfo.CurrentCulture);
    }

    public static bool IsWeekend(this DateTime dateTime)
    {
        return dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    public static bool IsBusinessHours(this DateTime dateTime, int startHour = 8, int endHour = 18)
    {
        return !dateTime.IsWeekend() && dateTime.Hour >= startHour && dateTime.Hour < endHour;
    }

    public static DateTime StartOfDay(this DateTime dateTime)
    {
        return dateTime.Date;
    }

    public static DateTime EndOfDay(this DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        return dateTime.StartOfWeek(startOfWeek).AddDays(7).AddTicks(-1);
    }

    public static DateTime StartOfMonth(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    public static DateTime EndOfMonth(this DateTime dateTime)
    {
        return dateTime.StartOfMonth().AddMonths(1).AddTicks(-1);
    }
}