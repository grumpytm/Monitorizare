using Microsoft.Extensions.Configuration;

namespace Monitorizare.Settings;

public class AppSettings : IAppSettings
{
    public IConfiguration Configuration { get; }

    public AppSettings() =>
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
            .Build();

    public Dictionary<string, string?> ServerDetails => GetDictionary("Server:Details");
    public IEnumerable<string> FilesList => GetList("Server:Files");
    public string DatabaseFile => GetValue("Database:File");
    public string LocalPath => GetValue("Server:Details:LocalPath");
    public string MigrationsPath => GetValue("Paths:Migrations");

    private Dictionary<string, string?> GetDictionary(string section) =>
        Configuration.GetSection(section).GetChildren()
            .ToDictionary(child => child.Key, child => child.Value);

    private IEnumerable<string> GetList(string section) =>
        Configuration.GetSection(section).GetChildren()
            .Select(child => child.Value)
            .Where(value => value.NotEmpty())
            .OfType<string>() ?? Enumerable.Empty<string>();

    private string GetValue(string section) =>
        Configuration.GetSection(section).Value ?? string.Empty;
}