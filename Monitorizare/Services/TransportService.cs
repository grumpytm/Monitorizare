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
        var fileList = _settings.GetFileList();
        if (!fileList.Any()) return;

        foreach (var (file, result) in await Task.WhenAll(fileList.Select(file => ProcessAndSaveLog(file))))
            await _logger.LogInfoAsync($"Processed '{file}' results: {result.count} valid lines, {result.affected} new records added to the database.");
    }

    private async Task<(string file, (int count, int affected))> ProcessAndSaveLog(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var records = fileName switch
        {
            "Incarcare.log" => await _logProcessor.TryParseLoadingAsync(filePath),
            "Descarcare.log" => await _logProcessor.TryParseUnloadingAsync(filePath),
            _ => throw new ArgumentException($"Invalid file name: {fileName ?? "<null>"}")
        };

        var affectedRows = await _database.SaveRecordsAsync(records);
        return (fileName, (records.Count(), affectedRows));
    }
}