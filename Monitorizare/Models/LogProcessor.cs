namespace Monitorizare.Models;

public class LogProcessor : ILogProcessor
{
    private readonly ILogger _logger;
    private readonly IAppSettings _settings;

    public LogProcessor(IAppSettings settings)
    {
        _settings = settings;
        _logger = LoggerFactory.CreateLogger();
    }

    public async Task<IEnumerable<TransportLog>> TryParseLoadingAsync(string filePath)
    {
        return await TryParseLogAsync(filePath, line =>
        {
            var parts = line.ProcessTimestamp();
            return new Incarcare(parts[0].ConvertTo<long>(), parts[1].ConvertTo<int>(), parts[2].ConvertTo<int>());
        });
    }

    public async Task<IEnumerable<TransportLog>> TryParseUnloadingAsync(string filePath)
    {
        return await TryParseLogAsync(filePath, line =>
        {
            var parts = line.ProcessTimestamp();
            return new Descarcare(parts[0].ConvertTo<long>(), parts[1].ConvertTo<int>(), parts[2].ConvertTo<int>(), parts[3].ConvertTo<int>(), parts[4].ConvertTo<int>());
        });
    }

    private async Task<IEnumerable<TransportLog>> TryParseLogAsync(string filePath, Func<string, TransportLog> parser)
    {
        var records = new List<TransportLog>();

        try
        {
            using StreamReader reader = File.OpenText(filePath);
            await reader.ReadLineAsync(); // skip header

            while (await reader.ReadLineAsync() is { } line)
            {
                var sanitizedLine = line.Sanitize();
                if (sanitizedLine.PassesValidation())
                {
                    records.Add(parser(sanitizedLine));
                }
                else
                {
                    await _logger.LogWarningAsync($"Error, while parsing {Path.GetFileName(filePath)} file, the `{sanitizedLine}` line didn't pass the validation and thus was ignored.");
                }
            }
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }

        return records;
    }
}