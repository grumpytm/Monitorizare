using System.Data.SQLite;
using Monitorizare.Settings;

namespace Monitorizare.Data;
public sealed class SingletonDB
{
    private readonly string _connectionString;

    private static readonly Lazy<SingletonDB> _instance = new(() => new SingletonDB(new AppSettings()));

    public static SingletonDB Instance => _instance.Value;

    private SingletonDB(IAppSettings settings)
    {
        var filePath = settings.GetDatabasePath() ?? "records.db";
        _connectionString = $"Data Source={filePath};Version=3;Compress=True;";
    }

    public SQLiteConnection CreateConnection() => new(_connectionString);
}