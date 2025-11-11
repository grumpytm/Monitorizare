using System.Collections.Immutable;

namespace Monitorizare.Models;

public class LogProcessor : ILogProcessor
{
    private readonly ILogger _logger;

    public LogProcessor() =>
        _logger = LoggerFactory.CreateLogger();

    private static readonly Dictionary<string, Func<string, TransportType>> Parsers = new()
    {
        // Filling records
        ["Incarcare.log"] = static line =>
            Parse(line, p => new Incarcare(p[0].ConvertTo<long>(), p[1].ConvertTo<int>(), p[2].ConvertTo<int>())),

        // Emptying records
        ["Descarcare.log"] = static line =>
            Parse(line, p => new Descarcare(p[0].ConvertTo<long>(), p[1].ConvertTo<int>(), p[2].ConvertTo<int>(), p[3].ConvertTo<int>(), p[4].ConvertTo<int>())),
    };

    public async Task<ImmutableList<TransportType>> TryParseLogAsync(string filePath)
    {
        var fileName = Path.GetFileName(filePath);

        if (!Parsers.TryGetValue(fileName, out var parser))
            throw new ArgumentException($"Error, file name: {fileName ?? "<null>"} doesn't have a valid parser!");

        var records = new List<TransportType>();

        await foreach (var line in GetSanitizedLines(filePath, _logger))
        {
            records.Add(parser(line));
        }

        return records.ToImmutableList();
    }

    private static async IAsyncEnumerable<string> GetSanitizedLines(string filePath, ILogger logger)
    {
        var fileName = Path.GetFileName(filePath);
        using var reader = File.OpenText(filePath);

        await reader.ReadLineAsync(); // skip header

        while (await reader.ReadLineAsync() is { } line)
        {
            var sanitizedLine = line.Sanitize();

            if (!sanitizedLine.MatchesPattern())
            {
                await logger.LogWarningAsync($"Error while parsing {fileName}: line `{sanitizedLine}` didn't pass validation and thus was dismissed.");
                continue;
            }

            yield return sanitizedLine;
        }
    }

    private static TransportType Parse(string line, Func<List<string>, TransportType> map) =>
        map(line.SplitIntoParts());
}