using Microsoft.Extensions.Configuration;

namespace Monitorizare.Settings;

public interface IAppSettings
{
    IConfiguration Configuration { get; }
    public Dictionary<string, string?> GetServerDetails();
    public IEnumerable<string> GetFileList();
    public Dictionary<string, string?> GetStoredQueries();
    public string? GetDatabasePath();
    public string GetLocalPath();
}