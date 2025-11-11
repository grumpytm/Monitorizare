namespace Monitorizare;

public static class CronTasks
{
    public static async Task SaveTransportLogs()
    {
        var settings = new AppSettings();
        var logProcessor = new LogProcessor();

        // Download files
# if !DEBUG
        var downloadService = new DownloadService(settings);
        await downloadService.DownloadFilesAsync();
# endif

        // Process and save logs to database
        var transportService = new TransportService(settings, logProcessor);
        await transportService.ProcessAndSaveLogsAsync();
    }
}