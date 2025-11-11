namespace Monitorizare.Data;

public class SQLiteDatabase : IDatabase
{
    public ILogger Logger { get; }
    public DatabaseMigrations Migrations { get; }
    public QueryLogs Logs { get; }
    public TransportRecordSaver Records { get; }
    public QueryTransport Transport { get; }
    public QueryMigrations QueryMigrations { get; }

    public SQLiteDatabase(ILogger logger, DatabaseMigrations migrations, QueryLogs logs, TransportRecordSaver records, QueryTransport transport, QueryMigrations queryMigrations)
    {
        Logger = logger;
        Migrations = migrations;
        Logs = logs;
        Records = records;
        Transport = transport;
        QueryMigrations = queryMigrations;
    }

    public Task ApplyMigrationsAsync() => Migrations.ApplyMigrationsAsync();
    public Task LogInfoAsync(string message, string? details = null) => Logger.LogInfoAsync(message, details);
    public Task LogWarningAsync(string message, string? details = null) => Logger.LogWarningAsync(message, details);
    public Task LogErrorAsync(string message, string? details = null) => Logger.LogErrorAsync(message, details);
    public Task LogExceptionAsync(Exception ex) => Logger.LogExceptionAsync(ex);
    public Task<int> SaveRecordsAsync(IEnumerable<TransportType> records) => Records.SaveRecordsAsync(records);
    public Task<IEnumerable<LogsDTO>> FetchLogsAsync() => Logs.FetchLogs();
    public Task<IEnumerable<DateTimeBoundsDTO>> GetMinMaxFor(string table) => Transport.GetMinMaxFor(table);
    public Task<IEnumerable<IncarcareDTO>> LoadIncarcareWithin(DateBounds bounds) => Transport.LoadIncarcareWithin(bounds);    
    public Task<IEnumerable<DescarcareDTO>> LoadDescarcareWithin(DateBounds bounds) => Transport.LoadDescarcareWithin(bounds);
    public Task<IEnumerable<LastDatesDTO>> GetMaxDates() => QueryTransport.GetMaxDates();
    public Task<IEnumerable<IncarcareDTO>> LastIncarcareRecords() => Transport.LastIncarcareRecords();
    public Task<IEnumerable<DescarcareDTO>> LastDescarcareRecords() => Transport.LastDescarcareRecords();
    public Task<IEnumerable<LongBoundsDTO>> GetTransportBounds() => QueryTransport.GetTransportBounds();
    public Task<IEnumerable<MigrationsDTO>> FetchMigrationsAsync() => QueryMigrations.FetchMigrationsAsync();
}