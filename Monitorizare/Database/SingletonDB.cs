using System.Data.SQLite;
using Monitorizare.Services;

namespace Monitorizare.Database;

internal sealed class SingletonDB
{
    private SingletonDB() { }

    private static readonly Lazy<SingletonDB> _lazyInstance = new(() => new());

    public static SingletonDB Instance =>
        _lazyInstance.Value;

    private string _connectionString { get => BuildConnectionString(); }

    string BuildConnectionString()
    {
        var filePath = SettingsProvider.GetDatabaseFile() ?? "database.db";
        return $"Data Source={filePath};Version=3;Compress=True;";
    }

    public SQLiteConnection GetNewConnection() =>
        new SQLiteConnection(_connectionString);
}