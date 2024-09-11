using System.Data.SQLite;

namespace Monitorizare.Database;

public sealed class SingletonDB
{
    private string _connectionString = @"Data Source=database.db;Version=3;Compress=True;";

    private SingletonDB() { }

    private static readonly Lazy<SingletonDB> _lazyInstance = new(() => new());

    public static SingletonDB Instance =>
        _lazyInstance.Value;

    public SQLiteConnection GetNewConnection()
    {
        var connection = new SQLiteConnection(_connectionString);
        return connection;
    }
}