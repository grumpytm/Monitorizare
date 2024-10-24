namespace Monitorizare.Models;

public record CustomSettings
{
    public Dictionary<string, string?> Schema { get; init; } = new();
    public string? File { get; init; }
}