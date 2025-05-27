namespace Monitorizare;

public class CronTasks
{

    private readonly ILogger _logger = new Logger();

    public async Task SaveTransportLogs()
    {
        var settings = new AppSettings();
        var logProcessor = new LogProcessor(settings);

        // Download files
        var downloadService = new DownloadService(settings);
        await downloadService.DownloadFilesAsync();

        // Process and save logs to database
        var transportService = new TransportService(settings, logProcessor);
        await transportService.ProcessAndSaveLogsAsync();
    }
}