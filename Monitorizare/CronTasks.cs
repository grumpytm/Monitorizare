namespace Monitorizare;

public class CronTasks
{
    public async Task SaveTransportLogs()
    {
        var settings = new AppSettings();
        var logProcessor = new LogProcessor(settings);

        // Download files
# if !DEBUG // Disabled for debugging purposes only
        var downloadService = new DownloadService(settings);
        await downloadService.DownloadFilesAsync();
#endif

        // Process and save logs to database
        var transportService = new TransportService(settings, logProcessor);
        await transportService.ProcessAndSaveLogsAsync();
    }
}