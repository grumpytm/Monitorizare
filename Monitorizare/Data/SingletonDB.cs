using System.Data.SQLite;

namespace Monitorizare.Data;

public sealed class SingletonDB
{
    private readonly string _connectionString;
    private static readonly Lazy<SingletonDB> _instance = new(() => new SingletonDB(new AppSettings()));
    public static SingletonDB Instance => _instance.Value;
    private SingletonDB(IAppSettings settings) => _connectionString = $"Data Source={settings.GetDatabasePath() ?? "records.db"}";
    public SQLiteConnection CreateConnection() => new(_connectionString);
}