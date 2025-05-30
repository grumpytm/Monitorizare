namespace Monitorizare.Data;

public interface IDatabase
{
    public Task ApplyMigrationsAsync();
    public Task LogInfoAsync(string message, string? details = null);
    public Task LogWarningAsync(string message, string? details = null);
    public Task LogErrorAsync(string message, string? details = null);
    public Task LogExceptionAsync(Exception ex);
    public Task<int> SaveRecordsAsync(IEnumerable<ITransport> records);
    public Task<IEnumerable<LogsDTO>> FetchLogsAsync();
    public Task<IEnumerable<DateTimeBoundsDTO>> GetMinMaxFor(string table);
    public Task<IEnumerable<IncarcareDTO>> LoadIncarcareWithin(long min, long max);
    public Task<IEnumerable<DescarcareDTO>> LoadDescarcareWithin(long min, long max);
    public Task<IEnumerable<LastDatesDTO>> GetMaxDates();
    public Task<IEnumerable<IncarcareDTO>> LastIncarcareRecords();
    public Task<IEnumerable<DescarcareDTO>> LastDescarcareRecords();
    public Task<IEnumerable<LongBoundsDTO>> GetTransportBounds();
}