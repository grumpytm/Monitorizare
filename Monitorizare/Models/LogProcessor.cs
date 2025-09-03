using System.Collections.Immutable;

namespace Monitorizare.Models;

public class LogProcessor : ILogProcessor
{
    private readonly ILogger _logger;

    public LogProcessor() =>
        _logger = LoggerFactory.CreateLogger();

    public async Task<ImmutableList<TransportType>> TryParseLogAsync(string filePath)
    {
        return await TryParseLogAsync<TransportType>(filePath, line =>
        {
            var parts = line.SplitIntoParts();
            var fileName = Path.GetFileName(filePath);

            return fileName switch
            {
                "Incarcare.log" => new Incarcare(parts[0].ConvertTo<long>(), parts[1].ConvertTo<int>(), parts[2].ConvertTo<int>()),
                "Descarcare.log" => new Descarcare(parts[0].ConvertTo<long>(), parts[1].ConvertTo<int>(), parts[2].ConvertTo<int>(), parts[3].ConvertTo<int>(), parts[4].ConvertTo<int>()),
                _ => throw new ArgumentException($"Invalid file name: {fileName ?? "<null>"}")
            };
        });
    }

    private async Task<ImmutableList<T>> TryParseLogAsync<T>(string filePath, Func<string, T> parser) where T : class
    {
        var records = new List<T>();
        var fileName = Path.GetFileName(filePath);

        try
        {
            using StreamReader reader = File.OpenText(filePath);
            await reader.ReadLineAsync(); // skip header

            while (await reader.ReadLineAsync() is { } line)
            {
                var sanitizedLine = line.Sanitize();
                if (sanitizedLine.PassesValidation())
                    records.Add(parser(sanitizedLine));
                else
                    await _logger.LogWarningAsync($"Error, while parsing {fileName} file, the `{sanitizedLine}` line didn't pass the validation and thus was ignored.");
            }
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }

        return records.ToImmutableList();
    }
}