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

    public async Task<IEnumerable<ITransport>> TryParseFileAsync(string filePath)
    {
        var records = new List<ITransport>();
        using StreamReader reader = File.OpenText(filePath);
        await reader.ReadLineAsync(); // skip header

        try
        {
            while (await reader.ReadLineAsync() is { } line)
            {
                var sanitizedLine = line.Sanitize();
                if (sanitizedLine.PassesValidation())
                    records.AddRange(sanitizedLine.ProcessTimestamp().ToTransportType(filePath));
            }
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }

        return records;
    }
}