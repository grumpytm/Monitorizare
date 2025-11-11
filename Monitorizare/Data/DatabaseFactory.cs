namespace Monitorizare.Data;

public class DatabaseFactory
{
    private static readonly Lazy<SQLiteDatabase> _instance = new(() =>
    {
        var logger = LoggerFactory.CreateLogger();
        var queryLogs = new QueryLogs();
        var transportRecordSaver = new TransportRecordSaver();
        var queryTransport = new QueryTransport();
        var databaseMigrations = new DatabaseMigrations(logger);
        var queryMigrations = new QueryMigrations();

        return new SQLiteDatabase(logger, databaseMigrations, queryLogs, transportRecordSaver, queryTransport, queryMigrations);
    });

    public static SQLiteDatabase GetDatabase() => _instance.Value;
}