using System.Diagnostics;
using Monitorizare.Database;

namespace Monitorizare.Services;

public class Logger
{
    private static readonly Lazy<Logger> _instance = new(() => new Logger());

    public static Logger Instance => _instance.Value;

    protected Logger() { }

    public async Task LogAsync(string message)
    {
#if DEBUG
        Debug.WriteLine($"Logger: {message}");
#endif
        await new SqliteLoggerSaver().SaveLog(message);
    }
}