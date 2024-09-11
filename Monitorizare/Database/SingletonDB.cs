using System.Data.SQLite;

namespace Monitorizare.Database;

public sealed class SingletonDB
{
    private string _connectionString = "Data Source=records.db;Version=3;Compress=True;";

    private readonly SQLiteConnection _connection;

    private SingletonDB() =>
        _connection = new SQLiteConnection(_connectionString);

    private static readonly Lazy<SingletonDB> _lazyInstance = new(() => new());

    public static SingletonDB Instance =>
        _lazyInstance.Value;

    public static SQLiteConnection GetInstance()
    {
        return Instance._connection;
    }
}