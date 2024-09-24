using Microsoft.Extensions.Configuration;

namespace Monitorizare.Services;

public class SettingsProvider
{
    private IConfiguration GetConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Settings.json", optional: true, reloadOnChange: true)
            .Build();
    }

    public string? GetDatabaseFile() =>
        GetConfig().GetSection("Database:File").Value;

    public string? GetValue(string section) =>
        GetConfig().GetSection(section).Value;

    public Dictionary<string, string?> GetQueries() =>
         GetConfig()
        .GetSection("Database:Schema")
        .GetChildren()
        .Where(x => x.Value != null)
        .ToDictionary(x => x.Key, x => x.Value);

    public string[] GetLogFiles() =>
         GetConfig()
        .GetSection("Server:Files")
        .GetChildren()
        .Where(x => x.Value != null)
        .Select(p => p.Value)
        .ToArray()!;

    public Dictionary<string, string?> GetServerData() =>
         GetConfig()
        .GetSection("Server:Details")
        .GetChildren()
        .Where(x => x.Value != null)
        .ToDictionary(x => x.Key, x => x.Value);
}