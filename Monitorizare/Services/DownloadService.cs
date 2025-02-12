using System.Net;
using FluentFTP;
using Monitorizare.Data;
using Monitorizare.Settings;

namespace Monitorizare.Services;

class DownloadService
{
    private readonly IAppSettings _settings;
    private readonly DatabaseService _database;

    public DownloadService(IAppSettings settings, DatabaseService database) =>
        (_settings, _database) = (settings, database);

    public async Task DownloadFilesAsync()
    {
        try
        {
            var filesList = _settings.GetFilePaths();
            var serverDetails = _settings.GetServerDetails();
            var localPath = Directory.GetCurrentDirectory();

            await using var client = new AsyncFtpClient
            {
                Host = serverDetails["Host"],
                Credentials = new NetworkCredential(serverDetails["User"], serverDetails["Pass"]),
                Config = { ConnectTimeout = 5000 }
            };

            await client.Connect();
            await client.SetWorkingDirectory(serverDetails["Path"]);

            foreach (var file in filesList)
            {
                await client.DownloadFile(Path.Combine(localPath, file), file);
            }
        }
        catch (Exception ex)
        {
            await _database.LogMessageAsync($"Download service error: {ex.Message}\nStackTrace: {ex.StackTrace}");
        }
    }
}