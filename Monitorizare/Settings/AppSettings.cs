using Microsoft.Extensions.Configuration;
using Monitorizare.Common;

namespace Monitorizare.Settings;

public class AppSettings : IAppSettings
{
    public IConfiguration Configuration { get; private set; }

    public AppSettings() =>
        Configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
        .Build();

    public Dictionary<string, string?> GetServerDetails() =>
        GetConfigurationDictionary("Server:Details");

    public IEnumerable<string> GetFileList()
    {
        var localPath = GetLocalPath();
        return GetConfigurationValues("Server:Files")
        .Select(file => Path.Combine(localPath, file))
        .Where(filePath => File.Exists(filePath))
        .OfType<string>();
    }

    public Dictionary<string, string?> GetStoredQueries() =>
        GetConfigurationDictionary("Database:Schema");

    public string GetDatabasePath() =>
        GetConfigurationValue("Database:File");

    public string GetLocalPath() =>
        GetConfigurationValue("Server:Details:LocalPath");

    private Dictionary<string, string?> GetConfigurationDictionary(string section) =>
        Configuration.GetSection(section).GetChildren()
        .ToDictionary(child => child.Key, child => child.Value);

    private IEnumerable<string> GetConfigurationValues(string section) =>
        Configuration.GetSection(section).GetChildren()
        .Select(child => child.Value)
        .Where(value => value.NotEmpty())
        .OfType<string>() ?? Enumerable.Empty<string>();

    private string GetConfigurationValue(string section) =>
        Configuration.GetSection(section).Value ?? string.Empty;
}