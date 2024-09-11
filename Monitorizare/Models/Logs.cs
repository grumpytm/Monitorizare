namespace Monitorizare.Models;

public record struct Logs(double Timestamp, byte Severity, string message);