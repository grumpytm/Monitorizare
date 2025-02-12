using Microsoft.Extensions.Configuration;

namespace Monitorizare.Settings;

public class AppSettings : IAppSettings
{
    public IConfiguration Configuration { get; private set; }

    public AppSettings()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    private bool IsValid(string? line) => !string.IsNullOrEmpty(line);

    public Dictionary<string, string?> GetServerDetails() =>
        Configuration
            .GetSection("Server:Details")
            .GetChildren()
            .ToDictionary(child => child.Key, child => child.Value);

   public IEnumerable<string> GetFilePaths() =>
        Configuration
            .GetSection("Server:Files")
            .GetChildren()
            .Select(child => child.Value)
            .Where(IsValid)!;

    public Dictionary<string, string?> GetStoredQueries() =>
        Configuration
            .GetSection("Database:Schema")
            .GetChildren()
            .ToDictionary(child => child.Key, child => child.Value);

    public string? GetDatabasePath() =>
        Configuration.GetSection("Database:File").Value;
}