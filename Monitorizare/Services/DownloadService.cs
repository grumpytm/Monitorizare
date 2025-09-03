using System.Net;
using FluentFTP;
using FluentFTP.Helpers;

namespace Monitorizare.Services;

class DownloadService
{
    private readonly IAppSettings _settings;
    private readonly ILogger _logger;

    public DownloadService(IAppSettings settings)
    {
        _settings = settings;
        _logger = LoggerFactory.CreateLogger();
    }

    public async Task DownloadFilesAsync()
    {
        var filesList = _settings.FilesList;
        if (!filesList.Any())
        {
            await _logger.LogErrorAsync("Error, missing file list in the configuration file, nothing to download.");
            return;
        }

        var settings = FTPSettings.Create(_settings);
        var localPath = Directory.GetCurrentDirectory().CombineLocalPath(settings.LocalPath);

        try
        {
            await using var client = new AsyncFtpClient
            {
                Host = settings.Host,
                Credentials = new NetworkCredential(settings.User, settings.Pass),
                Config = { ConnectTimeout = 5000, RetryAttempts = 0, DataConnectionConnectTimeout = 5000 }
            };

            await client.Connect();
            if (!await TrySetWorkingDirectory(client, settings.ServerPath)) return;

            foreach (var file in filesList)
                await DownloadFileOrLogError(client, localPath, file);

            await client.Disconnect();
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }
    }

    private async Task LogStatus(string file, FtpStatus status)
    {
        var message = status switch
        {
            FtpStatus.Success => $"File '{file}' was successfully downloaded.",
            FtpStatus.Failed => $"Failed to download '{file}' file.",
            _ => $"Unknown status {status} for {file} file."
        };

        await _logger.LogInfoAsync(message);
    }

    private async Task DownloadFileOrLogError(AsyncFtpClient client, string localPath, string file)
    {
        if (await client.FileExists(file))
        {
            var filePath = Path.Combine(localPath, file);
            var status = await client.DownloadFile(filePath, file, FtpLocalExists.Overwrite, FtpVerify.Retry);
            await LogStatus(file, status);
        }
        else
        {
            await _logger.LogErrorAsync($"Error, '{file}' doesn't exist on the FTP Server, check the 'Server > Files' section in the 'Settings.json' file.");
        }
    }

    private async Task<bool> TrySetWorkingDirectory(AsyncFtpClient client, string path)
    {
        if (string.IsNullOrEmpty(path) || !await client.DirectoryExists(path))
        {
            await _logger.LogErrorAsync($"Error, '{path}' path doesn't exist on the FTP Server. Check 'Server > Details' in 'Settings.json'.");
            return false;
        }

        await client.SetWorkingDirectory(path);
        return true;
    }
}