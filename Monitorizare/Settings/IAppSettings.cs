using Microsoft.Extensions.Configuration;

namespace Monitorizare.Settings;

public interface IAppSettings
{
    IConfiguration Configuration { get; }
    public Dictionary<string, string?> GetServerDetails();
    public IEnumerable<string> GetFilePaths();
    public Dictionary<string, string?> GetStoredQueries();
    public string? GetDatabasePath();
}