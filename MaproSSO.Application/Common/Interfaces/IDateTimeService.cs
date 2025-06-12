using System;

namespace MaproSSO.Application.Common.Interfaces
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
        DateTime Today { get; }
        DateOnly TodayDateOnly { get; }
        TimeOnly TimeNow { get; }

        DateTime ConvertToUserTimeZone(DateTime utcDateTime, string timeZone);
        DateTime ConvertToUtc(DateTime localDateTime, string timeZone);
        string GetUserTimeZone();
    }
}