using System.Data.Common;

namespace Monitorizare.Data.Queries;

public class QueryTransport : QueryDatabase
{
    private readonly ILogger _logger;
    private static DbConnection CreateConnection() => SingletonDB.Instance.CreateConnection();

    public QueryTransport() =>
        _logger = LoggerFactory.CreateLogger();

    public async Task<IEnumerable<DateTimeBoundsDTO>> GetMinMaxFor(string table)
    {
        await using var connection = CreateConnection();
        await connection.EnsureOpenAsync();
        var tableList = await connection.GetExistingTablesAsync();

        if (tableList.Contains(table))
        {
            var query = $"SELECT MIN(timestamp) AS min, MAX(timestamp) AS max FROM {table}";
            return await FetchAndMapAsync(query, ProcessTransportRange);
        }

        var now = DateTime.UtcNow;
        await _logger.LogErrorAsync($"Error, can't fetch range for unknown `{table}` table.");
        return new[] { new DateTimeBoundsDTO(now, now) };
    }

    public async Task<IEnumerable<IncarcareDTO>> LoadIncarcareWithin(DateBounds bounds) =>
        await LoadRecordsAsync("incarcare", "timestamp, siloz, greutate", ProcessIncarcareData, bounds);

    public async Task<IEnumerable<DescarcareDTO>> LoadDescarcareWithin(DateBounds bounds) =>
        await LoadRecordsAsync("descarcare", "timestamp, siloz, greutate, hala, buncar", ProcessDescarcareData, bounds);

    public async Task<IEnumerable<IncarcareDTO>> LastIncarcareRecords() =>
        await LoadRecordsAsync("incarcare", "timestamp, siloz, greutate", ProcessIncarcareData);

    public async Task<IEnumerable<DescarcareDTO>> LastDescarcareRecords() =>
        await LoadRecordsAsync("descarcare", "timestamp, siloz, greutate, hala, buncar", ProcessDescarcareData);

    private async Task<IEnumerable<T>> LoadRecordsAsync<T>(string table, string columns, Func<DbDataReader, T> mapFunc, DateBounds bounds = default)
    {
        try
        {
            string whereClause = (bounds.Min == 0 || bounds.Max == 0)
                ? $"strftime('%Y-%m-%d', datetime(timestamp, 'unixepoch', 'localtime')) IN (SELECT strftime('%Y-%m-%d', datetime(MAX(timestamp), 'unixepoch', 'localtime')) FROM {table})"
                : $" {"timestamp".ToStrftime()} BETWEEN {bounds.Min.ToStrftime()} AND {bounds.Max.ToStrftime()}";

            var query = $"SELECT {columns} FROM {table} WHERE {whereClause} ORDER BY timestamp ASC";
            return await FetchAndMapAsync(query, mapFunc);
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
            return Enumerable.Empty<T>();
        }
    }

    private static DateTimeBoundsDTO ProcessTransportRange(DbDataReader reader)
    {
        var min = reader.GetValueOrDefault("min", v => v.ConvertTo<long>().ExtractDateTime());
        var max = reader.GetValueOrDefault("max", v => v.ConvertTo<long>().ExtractDateTime());
        return new(min, max);
    }


    private static IncarcareDTO ProcessIncarcareData(DbDataReader reader)
    {
        var (date, time) = reader["timestamp"].ConvertTo<long>().ExtractDateAndTime();
        var siloz = reader["siloz"].ConvertTo<int>();
        var greutate = reader["greutate"].ConvertTo<int>();
        return new(date, time, siloz, greutate);
    }

    private static DescarcareDTO ProcessDescarcareData(DbDataReader reader)
    {
        var (date, time) = reader["timestamp"].ConvertTo<long>().ExtractDateAndTime();
        var siloz = reader["siloz"].ConvertTo<int>();
        var greutate = reader["greutate"].ConvertTo<int>();
        var hala = reader["hala"].ConvertTo<int>();
        var buncar = reader["buncar"].ConvertTo<int>();
        return new(date, time, siloz, greutate, hala, buncar);
    }

    public static async Task<IEnumerable<LastDatesDTO>> GetMaxDates()
    {
        var query = "SELECT MAX(timestamp) AS Last, 'Incarcare' AS Type FROM incarcare UNION ALL SELECT MAX(timestamp) AS Last, 'Descarcare' AS Type FROM descarcare";
        return await FetchAndMapAsync(query, ProcessLastRanges);
    }

    public static async Task<IEnumerable<LongBoundsDTO>> GetTransportBounds()
    {
        var query = "SELECT MIN(timestamp) AS Min, MAX(timestamp) AS Max, 'Incarcare' AS Type FROM incarcare UNION ALL SELECT MIN(timestamp) AS Min, MAX(timestamp) AS Max, 'Descarcare' AS Type FROM descarcare";
        return await FetchAndMapAsync(query, ProcessBounds);
    }

    private static LastDatesDTO ProcessLastRanges(DbDataReader reader)
    {
        var type = reader["Type"].ConvertTo<string>();
        var last = reader["Last"].ConvertTo<long>().ExtractDateTime();
        return new(type, last);
    }

    private static LongBoundsDTO ProcessBounds(DbDataReader reader)
    {
        var type = reader["type"].ConvertTo<string>();
        var min = reader.GetValueOrDefault<long>("min");
        var max = reader.GetValueOrDefault<long>("max");
        return new(type, min, max);
    }
}
