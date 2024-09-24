using System.Data.SQLite;
using Monitorizare.Services;

namespace Monitorizare.Database;

internal sealed class SingletonDB
{
    private SettingsProvider _settingsProvider;
    private string _filePath;

    private SingletonDB()
    {
        _settingsProvider = new SettingsProvider();
        _filePath = _settingsProvider.GetDatabaseFile() ?? "database.db";
    }

    private static readonly Lazy<SingletonDB> _lazyInstance = new(() => new());

    public static SingletonDB Instance =>
        _lazyInstance.Value;

    private string _connectionString =>
        $"Data Source={_filePath};Version=3;Compress=True;";

    public SQLiteConnection CreateConnection() =>
        new SQLiteConnection(_connectionString);
}