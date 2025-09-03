namespace Monitorizare.Data;

public interface IDatabase
{
    // Migrations
    Task ApplyMigrationsAsync();

    // Logging
    Task LogInfoAsync(string message, string? details = null);
    Task LogWarningAsync(string message, string? details = null);
    Task LogErrorAsync(string message, string? details = null);
    Task LogExceptionAsync(Exception ex);

    // Transport records
    Task<int> SaveRecordsAsync(IEnumerable<TransportType> records);

    // Logs queries
    Task<IEnumerable<LogsDTO>> FetchLogsAsync();

    // Transport queries
    Task<IEnumerable<DateTimeBoundsDTO>> GetMinMaxFor(string table);
    Task<IEnumerable<IncarcareDTO>> LoadIncarcareWithin(DateBounds bounds);
    Task<IEnumerable<DescarcareDTO>> LoadDescarcareWithin(DateBounds bounds);
    Task<IEnumerable<LastDatesDTO>> GetMaxDates();
    Task<IEnumerable<IncarcareDTO>> LastIncarcareRecords();
    Task<IEnumerable<DescarcareDTO>> LastDescarcareRecords();
    Task<IEnumerable<LongBoundsDTO>> GetTransportBounds();
}