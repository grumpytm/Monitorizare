using System.Globalization;
using System.Text.RegularExpressions;

namespace Monitorizare.Records;

public class TransportLogsParser
{
    private readonly char[] _trimChars = new char[] { '\t', ',', ' ' };

    private readonly string _pattern = @"^\d{2}/\d{2}/\d{4},\d{2}:\d{2},\d+,\d+(,\d+,\d+)?$";

    public async Task<IEnumerable<string>> ParseLogAsync(string filePath)
    {
        string[] lines = await File.ReadAllLinesAsync(filePath);
        return lines.Skip(1).Select(Filtered).Where(MatchesPattern);
    }

    private string Filtered(string content) =>
    content.Replace(";", ",").TrimEnd(_trimChars);

    private bool MatchesPattern(string line) =>
        !string.IsNullOrEmpty(line) && Regex.IsMatch(line, _pattern);

    public int GetFrom(string[] collection, int index)
    {
        if (collection == null || index >= collection.Length) return 0;
        return int.TryParse(collection[index], out int no) ? no : 0;
    }

    public double GetTimestamp(string date, string time) =>
        DateTime.TryParseExact($"{date} {time}", "dd/MM/yyyy H:mm",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate)
        ? parsedDate.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds
        : 0;

    public (double, int, int, int, int) ExtractTransportDataFrom(string[] collection)
    {
        int count = collection.Count();

        double timestamp = DateTime
            .ParseExact($"{collection[0]} {collection[1]}", "dd/MM/yyyy H:mm", CultureInfo.InvariantCulture)
            .ToUniversalTime()
            .Subtract(new DateTime(1970, 1, 1))
            .TotalSeconds;

        int siloz = GetFrom(collection, 2);
        int greutate = GetFrom(collection, 3);
        int hala = count > 4 ? GetFrom(collection, 4) : 0;
        int buncar = count > 4 ? GetFrom(collection, 5) : 0;

        return (timestamp, siloz, greutate, hala, buncar);
    }
}