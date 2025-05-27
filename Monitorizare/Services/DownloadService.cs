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

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            Console.WriteLine($"{DateTime.UtcNow} [Unhandled exception] - {args.ExceptionObject}");
        };
    }

    public async Task DownloadFilesAsync()
    {
        var filesList = await GetFileList();
        if (!filesList.Any()) return;

        var settings = new FTPServerSettings(_settings);
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
            await client.SetWorkingDirectory(settings.ServerPath);

            foreach (var file in filesList)
            {
                var status = await client.DownloadFile(Path.Combine(localPath, file), file, FtpLocalExists.Overwrite, FtpVerify.Retry);
                await LogStatus(file, status);
            }
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }
    }

    private async Task<IEnumerable<string>> GetFileList()
    {
        var fileList = _settings.GetFileList();

        if (fileList == null || !fileList.Any())
        {
            await _logger.LogErrorAsync("Error, corrupted or missing file list in configuration file, nothing to download.");
            return Enumerable.Empty<string>();
        }

        return fileList.Cast<string>();
    }

    private async Task LogStatus(string file, FtpStatus status)
    {
        var message = status switch
        {
            FtpStatus.Success => $"File {file} successfully downloaded.",
            FtpStatus.Failed => $"Failed to download {file} file.",
            _ => $"Unknown status {status} for {file} file."
        };

        await _logger.LogInfoAsync(message);
    }
}