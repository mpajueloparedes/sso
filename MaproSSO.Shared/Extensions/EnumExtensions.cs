using MaproSSO.Shared.Models;
using System.ComponentModel;
using System.Reflection;

namespace MaproSSO.Shared.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttribute?.Description ?? value.ToString();
    }

    //public static Dictionary<int, string> GetEnumDictionary<T>() where T : Enum
    //{
    //    return Enum.GetValues<T>()
    //        .ToDictionary(e => Convert.ToInt32(e), e => e.GetDescription());
    //}

    //public static List<SelectOption> GetSelectOptions<T>() where T : Enum
    //{
    //    return Enum.GetValues<T>()
    //        .Select(e => new SelectOption(Convert.ToInt32(e).ToString(), e.GetDescription()))
    //        .ToList();
    //}

    public static T ParseEnum<T>(string value, T defaultValue = default) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, true, out var result))
        {
            return result;
        }
        return defaultValue;
    }
}