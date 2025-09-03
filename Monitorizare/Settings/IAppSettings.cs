using Microsoft.Extensions.Configuration;

namespace Monitorizare.Settings;

public interface IAppSettings
{
    IConfiguration Configuration { get; }

    public Dictionary<string, string?> ServerDetails { get; }
    public IEnumerable<string> FilesList { get; }
    public string DatabaseFile { get; }
    public string LocalPath { get; }
    public string MigrationsPath { get; }
}