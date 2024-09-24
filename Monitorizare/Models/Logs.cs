namespace Monitorizare.Models;

public record struct Logs(double Timestamp, byte Severity, string Message)
{
    public static Logs Create(double timestamp, byte severity, string message) =>
        new(timestamp, severity, message);
};