using System.Globalization;
using System.Text.RegularExpressions;

namespace Monitorizare.Models;

public static class LogProcessorExtensions
{
    private static readonly char[] _trimChars = new char[] { '\t', ',', ' ' };
    private static readonly Regex MatchesPattern = new(@"^\d{2}/\d{2}/\d{4},\d{2}:\d{2},\d+,\d+(,\d+,\d+)?$");

    public static string Sanitize(this string line) =>
        line.Replace(";", ",").TrimEnd(_trimChars);

    public static bool PassesValidation(this string line) =>
        line.NotEmpty() && MatchesPattern.IsMatch(line);

    public static List<string> ProcessTimestamp(this string line)
    {
        var parts = line.Split(',');
        var timestamp = parts.Take(2).ParseDateTime().ToUnixTimestamp();
        return new[] { $"{timestamp}" }.Concat(parts.Skip(2)).ToList();
    }

    private static DateTime ParseDateTime(this IEnumerable<string> parts) =>
        DateTime.TryParseExact($"{string.Join(" ", parts)}", "dd/MM/yyyy HH:mm",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue)
            ? dateValue : DateTime.UtcNow;

    private static long ToUnixTimestamp(this DateTime dateTime) =>
        new DateTimeOffset(dateTime).ToUnixTimeSeconds();
}