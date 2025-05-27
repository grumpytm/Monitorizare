namespace Monitorizare.Data;

public class LoggerFactory
{
    private static readonly Lazy<ILogger> _instance = new(() => new Logger());
    public static ILogger CreateLogger() => _instance.Value;
}