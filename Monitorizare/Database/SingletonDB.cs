using System.Data.SQLite;
using Monitorizare.Settings;

namespace Monitorizare.Database;

public sealed class SingletonDB
{
    private readonly string _filePath;

    private string _connectionString =>
        $"Data Source={_filePath};Version=3;Compress=True;";

    private static readonly Lazy<SingletonDB> _instance = new(() =>
        new SingletonDB());

    private readonly Lazy<SettingsFileParser> _settings = new(() =>
        new SettingsFileParser());

    public static SingletonDB Instance =>
        _instance.Value;

    public SQLiteConnection CreateConnection() =>
        new SQLiteConnection(_connectionString);

    private SingletonDB()
    {
        string? file = new SettingsFileParser().GetSettings().File;
        _filePath = string.IsNullOrWhiteSpace(file) ? "records.db" : file;
    }
}