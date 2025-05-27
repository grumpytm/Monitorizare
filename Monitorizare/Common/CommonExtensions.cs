using System.Data;

namespace Monitorizare.Common;

public static class CommonExtensions
{
    public static bool NotEmpty(this string? value) =>
        !string.IsNullOrEmpty(value);

    public static bool IsLessThan(this int value, int expected) =>
        value < expected;

    public static bool IsWithin(this int value, int min, int max) =>
        value >= min && value <= max;

    public static IEnumerable<T> GetVisibleControlsOfType<T>(this Control parent) where T : Control =>
        parent.Controls.OfType<T>().Where(c => c.Visible);

    public static List<string> ExtractUniqueValuesFor<T>(this IEnumerable<T> content, Control comboBox) where T : class
    {
        var firstItem = new List<string> { "-" };
        var name = comboBox.Name.Replace("ComboBox", "");
        var prop = typeof(T).GetProperty(name);
        if (prop == null) return firstItem;

        var distinctItems = content
            .Select(row => prop.GetValue(row)?.ToString() ?? string.Empty)
            .Where(NotEmpty)
            .Distinct()
            .OrderBy(val => int.TryParse(val, out var num) ? num : int.MaxValue);

        return firstItem.Concat(distinctItems).ToList();
    }

    public static IEnumerable<string> ExtractDistinct<T>(this IEnumerable<T> content, string name)
    {
        var prop = typeof(T).GetProperty(name) ?? throw new ArgumentException($"Property '{name}' not found on type {typeof(T).Name}");
        return content.Select(x => prop.GetValue(x)?.ToString() ?? string.Empty)
            .Where(NotEmpty)
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    public static (DateTime, DateTime) GetDayBounds(this DateTime date) =>
        (date.Date, date.Date.AddDays(1).AddTicks(-1)); // StartOfDay & EndOfDay

    public static (long, long) ToUnixTime(this (DateTime, DateTime) bounds) =>
        (bounds.Item1.ToUnixTime(), bounds.Item2.ToUnixTime());
}