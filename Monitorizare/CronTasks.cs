using Monitorizare.Data;
using Monitorizare.Records;
using Monitorizare.Services;
using Monitorizare.Settings;

namespace Monitorizare;

class CronTasks
{
    public async Task SaveTransportLogs()
    {
        var settings = new AppSettings();
        var database = new SQLiteDatabase(settings);
        var logProcessor = new LogProcessor();

        // Download files
        // var downloadService = new DownloadService(settings, database);
        // await downloadService.DownloadFilesAsync();

        // Parse & save
        var transportService = new TransportService(settings, database, logProcessor);
        var (count, affected) = await transportService.ParseAndSave();

        // Log results
        var (pluralCount, pluralAffected) = (count.Pluralize(), affected.Pluralize());
        await database.LogMessageAsync($"Transport service report: {count} valid record{pluralCount} found, {affected} record{pluralAffected} were added to the database.");
    }
}

public static class StringExtensions
{
    public static string Pluralize(this int count) =>
        count == 1 ? "" : "s";
}