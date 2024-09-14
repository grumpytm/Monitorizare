using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Xml;

namespace Monitorizare.Services;

public class SettingsProvider
{
    private static IConfiguration GetConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Settings.json", optional: true, reloadOnChange: true)
            .Build();
    }

    public static string? GetDatabaseFile() =>
        GetConfig().GetSection("Database:File").Value;

    public static string? GetValue(string section) =>
        GetConfig().GetSection(section).Value;

    public static Dictionary<string, string?> GetQueries() =>
         GetConfig()
        .GetSection("Database:Schema")
        .GetChildren()
        .Where(x => x.Value != null)
        .ToDictionary(x => x.Key, x => x.Value);

    public static string[] GetLogFiles() =>
         GetConfig()
        .GetSection("Server:Files")
        .GetChildren()
        .Where(x => x.Value != null)
        .Select(p => p.Value)
        .ToArray()!;
}