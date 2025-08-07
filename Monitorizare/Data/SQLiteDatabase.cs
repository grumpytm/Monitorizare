namespace Monitorizare.Data;

public class SQLiteDatabase : IDatabase
{
    private readonly ILogger _logger;
    private readonly DatabaseMigrations _databaseMigrations;
    private readonly QueryLogs _queryLogs;
    private readonly TransportRecordSaver _transportSaver;
    private readonly QueryTransport _queryTransport;

    public SQLiteDatabase(ILogger logger, DatabaseMigrations databaseMigrations, QueryLogs queryLogs, TransportRecordSaver transportSaver, QueryTransport queryTransport)
    {
        _logger = logger;
        _databaseMigrations = databaseMigrations;
        _queryLogs = queryLogs;
        _transportSaver = transportSaver;
        _queryTransport = queryTransport;
    }

    public async Task ApplyMigrationsAsync() =>
        await _databaseMigrations.ApplyMigrationsAsync();

    public async Task LogInfoAsync(string message, string? details = null) =>
        await _logger.LogInfoAsync(message, details);

    public async Task LogWarningAsync(string message, string? details = null) =>
        await _logger.LogWarningAsync(message, details);

    public async Task LogErrorAsync(string message, string? details = null) =>
        await _logger.LogErrorAsync(message, details);

    public async Task LogExceptionAsync(Exception ex) =>
        await _logger.LogExceptionAsync(ex);

    public async Task<int> SaveRecordsAsync(IEnumerable<TransportLog> records) =>
        await _transportSaver.SaveRecordsAsync(records);

    public async Task<IEnumerable<LogsDTO>> FetchLogsAsync() =>
        await _queryLogs.FetchLogs();

    public async Task<IEnumerable<DateTimeBoundsDTO>> GetMinMaxFor(string table) =>
        await _queryTransport.GetMinMaxFor(table);

    public async Task<IEnumerable<IncarcareDTO>> LoadIncarcareWithin(DateBounds bounds) =>
        await _queryTransport.LoadIncarcareWithin(bounds);

    public async Task<IEnumerable<DescarcareDTO>> LoadDescarcareWithin(DateBounds bounds) =>
        await _queryTransport.LoadDescarcareWithin(bounds);

    public async Task<IEnumerable<LastDatesDTO>> GetMaxDates() =>
        await _queryTransport.GetMaxDates();

    public async Task<IEnumerable<IncarcareDTO>> LastIncarcareRecords() =>
        await _queryTransport.LastIncarcareRecords();

    public async Task<IEnumerable<DescarcareDTO>> LastDescarcareRecords() =>
        await _queryTransport.LastDescarcareRecords();

    public async Task<IEnumerable<LongBoundsDTO>> GetTransportBounds() =>
        await _queryTransport.GetTransportBounds();
}