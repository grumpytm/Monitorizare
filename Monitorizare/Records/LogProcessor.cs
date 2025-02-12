using System.Text.RegularExpressions;

namespace Monitorizare.Records;

public class LogProcessor : ILogProcessor
{
    public async Task<IEnumerable<Queries>> ProcessLogAsync(string filePath) =>
        BuildQueries(await ParseLogAsync(filePath));

    private async Task<IEnumerable<Transport>> ParseLogAsync(string filePath) =>
        (await File.ReadAllLinesAsync(filePath))
            .Skip(1) // skip header
            .Select(Filtered)
            .Where(MatchesPattern)
            .Select(ProcessLine)
            .SelectMany(TransportRecords);

    private IEnumerable<Queries> BuildQueries(IEnumerable<Transport> records)
    {
        return records.Select(record => record switch
        {
            Incarcare => new Queries($"INSERT OR IGNORE INTO incarcare (data, siloz, greutate) VALUES ('{record.Data}', '{record.Siloz}', '{record.Greutate}')"),
            Descarcare => new Queries($"INSERT OR IGNORE INTO descarcare (data, siloz, greutate, hala, buncar) VALUES ('{record.Data}', '{record.Siloz}', '{record.Greutate}', '{record.Hala}', '{record.Buncar}')"),
            _ => throw new NotSupportedException($"Error, {record.GetType().Name} is an unknown record type, can't process it.")
        });
    }

    private string ProcessLine(string line)
    {
        var parts = line.Split(',');

        if (DateTime.TryParseExact($"{parts[0]} {parts[1]}", "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
        {
            long timestamp = new DateTimeOffset(dateTime).ToUnixTimeSeconds();

            return $"{timestamp},{string.Join(",", parts.Skip(2))}";
        }

        return string.Empty;
    }

    private IEnumerable<Transport> TransportRecords(string line)
    {
        var parts = line.Split(',');

        return parts.Length switch
        {

            3 => new[] { new Incarcare(long.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])) },
            5 => new[] { new Descarcare(long.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4])) },
            _ => Enumerable.Empty<Transport>()
        };
    }

    private readonly char[] _trimChars = new char[] { '\t', ',', ' ' };

    private readonly string _pattern = @"^\d{2}/\d{2}/\d{4},\d{2}:\d{2},\d+,\d+(,\d+,\d+)?$";

    private string Filtered(string content) => content.Replace(";", ",").TrimEnd(_trimChars);

    private bool MatchesPattern(string line) => !string.IsNullOrEmpty(line) && Regex.IsMatch(line, _pattern);
}