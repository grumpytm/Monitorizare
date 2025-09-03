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
            var query = $"SELECT MIN(data) AS min, MAX(data) AS max FROM {table}";
            return await FetchAndMapAsync(query, ProcessTransportRange);
        }

        var now = DateTime.UtcNow;
        await _logger.LogErrorAsync($"Error, can't fetch range for unknown `{table}` table.");
        return new[] { new DateTimeBoundsDTO(now, now) };
    }

    public async Task<IEnumerable<IncarcareDTO>> LoadIncarcareWithin(DateBounds bounds) =>
        await LoadRecordsAsync("incarcare", "data, siloz, greutate", ProcessIncarcareData, bounds);

    public async Task<IEnumerable<DescarcareDTO>> LoadDescarcareWithin(DateBounds bounds) =>
        await LoadRecordsAsync("descarcare", "data, siloz, greutate, hala, buncar", ProcessDescarcareData, bounds);

    public async Task<IEnumerable<IncarcareDTO>> LastIncarcareRecords() =>
        await LoadRecordsAsync("incarcare", "data, siloz, greutate", ProcessIncarcareData);

    public async Task<IEnumerable<DescarcareDTO>> LastDescarcareRecords() =>
        await LoadRecordsAsync("descarcare", "data, siloz, greutate, hala, buncar", ProcessDescarcareData);

    private async Task<IEnumerable<T>> LoadRecordsAsync<T>(string table, string columns, Func<DbDataReader, T> mapFunc, DateBounds bounds = default)
    {
        try
        {
            string whereClause = (bounds.Min == 0 || bounds.Max == 0)
                ? $"strftime('%Y-%m-%d', datetime(data, 'unixepoch', 'localtime')) IN (SELECT strftime('%Y-%m-%d', datetime(MAX(data), 'unixepoch', 'localtime')) FROM {table})"
                : $" {"data".ToStrftime()} BETWEEN {bounds.Min.ToStrftime()} AND {bounds.Max.ToStrftime()}";

            var query = $"SELECT {columns} FROM {table} WHERE {whereClause} ORDER BY data ASC";
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
        return new DateTimeBoundsDTO(min, max);
    }

    private static IncarcareDTO ProcessIncarcareData(DbDataReader reader)
    {
        var (date, time) = reader["data"].ConvertTo<long>().ExtractDateAndTime();
        var siloz = reader["siloz"].ConvertTo<int>();
        var greutate = reader["greutate"].ConvertTo<int>();
        return new IncarcareDTO(date, time, siloz, greutate);
    }

    private static DescarcareDTO ProcessDescarcareData(DbDataReader reader)
    {
        var (date, time) = reader["data"].ConvertTo<long>().ExtractDateAndTime();
        var siloz = reader["siloz"].ConvertTo<int>();
        var greutate = reader["greutate"].ConvertTo<int>();
        var hala = reader["hala"].ConvertTo<int>();
        var buncar = reader["buncar"].ConvertTo<int>();
        return new DescarcareDTO(date, time, siloz, greutate, hala, buncar);
    }

    public async Task<IEnumerable<LastDatesDTO>> GetMaxDates()
    {
        var query = $"SELECT MAX(data) AS Last, 'Incarcare' AS Type FROM incarcare UNION ALL SELECT MAX(data) AS Last, 'Descarcare' AS Type FROM descarcare";
        return await FetchAndMapAsync(query, ProcessLastRanges);
    }

    public async Task<IEnumerable<LongBoundsDTO>> GetTransportBounds()
    {
        var query = $"SELECT MIN(data) AS Min, MAX(data) AS Max, 'Incarcare' AS Type FROM incarcare UNION ALL SELECT MIN(data) AS Min, MAX(data) AS Max, 'Descarcare' AS Type FROM descarcare";
        return await FetchAndMapAsync(query, ProcessBounds);
    }

    private static LastDatesDTO ProcessLastRanges(DbDataReader reader)
    {
        var type = reader["Type"].ConvertTo<string>();
        var last = reader["Last"].ConvertTo<long>().ExtractDateTime();
        return new LastDatesDTO(type, last);
    }

    private static LongBoundsDTO ProcessBounds(DbDataReader reader)
    {
        var type = reader["type"].ConvertTo<string>();
        var min = reader.GetValueOrDefault<long>("min");
        var max = reader.GetValueOrDefault<long>("max");
        return new LongBoundsDTO(type, min, max);
    }
}
