namespace Monitorizare.Services;

class TransportService
{
    private readonly IAppSettings _settings;
    private readonly ILogProcessor _logProcessor;
    private readonly ILogger _logger;
    private readonly SQLiteDatabase _database = DatabaseFactory.GetDatabase();

    public TransportService(IAppSettings settings, ILogProcessor logProcessor)
    {
        _settings = settings;
        _logProcessor = logProcessor;
        _logger = LoggerFactory.CreateLogger();
    }

    public async Task ProcessAndSaveLogsAsync()
    {
        var fileList = _settings.FilesList;

        if (!fileList.Any())
        {
            await _logger.LogErrorAsync("Error, missing file list in configuration file, nothing to process.");
            return;
        }

        foreach (var (file, output) in await Task.WhenAll(fileList.Select(file => ProcessAndSaveLog(file))))
        {
            await (output.count == 0
            ? _logger.LogWarningAsync($"File '{file}' hasn't been processed because it's missing.")
            : _logger.LogInfoAsync($"Processed '{file}' results: {output.count} valid lines, {output.affected} new records added to the database."));
        }
    }

    private async Task<(string file, (int count, int affected))> ProcessAndSaveLog(string fileName)
    {
        var filePath = Path.Combine(_settings.LocalPath, fileName);

        if (!File.Exists(filePath))
            return (fileName, (0, 0));

        var records = await _logProcessor.TryParseLogAsync(filePath);
        var affectedRows = await _database.SaveRecordsAsync(records);

        return (fileName, (records.Count, affectedRows));
    }
}