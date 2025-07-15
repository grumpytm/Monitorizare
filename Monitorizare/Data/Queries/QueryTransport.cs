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
        if (ITransport.TableList.Contains(table))
        {
            var query = $"SELECT MIN(data) AS min, MAX(data) AS max FROM {table}";
            return await FetchAndMapAsync(query, ProcessTransportRange);
        }

        var now = DateTime.UtcNow;
        await _logger.LogErrorAsync($"Error, can't fetch range for unknown `{table}` table.");
        return new[] { new DateTimeBoundsDTO(now, now) };
    }

    public async Task<IEnumerable<IncarcareDTO>> LoadIncarcareWithin(long min, long max) =>
        await LoadRecordsAsync("incarcare", "data, siloz, greutate", ProcessIncarcareData, min, max);

    public async Task<IEnumerable<DescarcareDTO>> LoadDescarcareWithin(long min, long max) =>
        await LoadRecordsAsync("descarcare", "data, siloz, greutate, hala, buncar", ProcessDescarcareData, min, max);

    public async Task<IEnumerable<IncarcareDTO>> LastIncarcareRecords() =>
        await LoadRecordsAsync("incarcare", "data, siloz, greutate", ProcessIncarcareData);

    public async Task<IEnumerable<DescarcareDTO>> LastDescarcareRecords() =>
        await LoadRecordsAsync("descarcare", "data, siloz, greutate, hala, buncar", ProcessDescarcareData);

    public IAsyncEnumerable<IDictionary<string, object>> ExportIncarcareBetween(long min, long max) => ExportTableAsync("incarcare", min, max, reader =>
            {
                var (date, time) = reader["data"].ConvertTo<long>().ExtractDateAndTime();
                return new Dictionary<string, object>
                {
                    ["Data"] = date.ToString("dd.MM.yyyy"),
                    ["Ora"] = time,
                    ["Siloz"] = reader["siloz"].ConvertTo<int>(),
                    ["Greutate"] = reader["greutate"].ConvertTo<int>()
                };
            });

    public IAsyncEnumerable<IDictionary<string, object>> ExportDescarcareBetween(long min, long max) => ExportTableAsync("descarcare", min, max, reader =>
    {
        var (date, time) = reader["data"].ConvertTo<long>().ExtractDateAndTime();
        return new Dictionary<string, object>
        {
            ["Data"] = date.ToString("dd.MM.yyyy"),
            ["Ora"] = time,
            ["Siloz"] = reader["siloz"].ConvertTo<int>(),
            ["Greutate"] = reader["greutate"].ConvertTo<int>(),
            ["Hala"] = reader["hala"].ConvertTo<int>(),
            ["Buncar"] = reader["buncar"].ConvertTo<int>()
        };
    });

    private async Task<IEnumerable<T>> LoadRecordsAsync<T>(string table, string columns, Func<DbDataReader, T> mapFunc, long min = 0, long max = 0)
    {
        try
        {
            string whereClause = (min == 0 || max == 0)
                ? $"strftime('%Y-%m-%d', datetime(data, 'unixepoch', 'localtime')) IN (SELECT strftime('%Y-%m-%d', datetime(MAX(data), 'unixepoch', 'localtime')) FROM {table})"
                : $" {"data".ToStrftime()} BETWEEN {min.ToStrftime()} AND {max.ToStrftime()}";

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
        var now = DateTime.UtcNow;

        DateTime GetValue(string column) =>
            reader[column] is DBNull ? now : reader[column].ConvertTo<long>().ExtractDateTime();

        return new DateTimeBoundsDTO(GetValue("min"), GetValue("max"));
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

    public async Task<IEnumerable<LastDatesDTO>> GetMaxDatesFor(string table)
    {
        var query = $"SELECT MAX(data) AS Last FROM {table}";
        return await FetchAndMapAsync(query, ProcessLastRange);
    }

    public async Task<IEnumerable<LastDatesDTO>> GetMaxDates()
    {
        var query = $"SELECT MAX(data) AS Last, 'Incarcare' AS Type FROM incarcare UNION ALL SELECT MAX(data) AS Last, 'Descarcare' AS Type FROM descarcare";
        return await FetchAndMapAsync(query, ProcessLastRanges);
    }

    public async Task<IEnumerable<LongBoundsDTO>> GetTransportBounds()
    {
        var query = $"SELECT MIN(data) AS Min, MAX(data) AS Max, 'Incarcare' AS Type FROM incarcare UNION ALL  SELECT MIN(data) AS Min, MAX(data) AS Max, 'Descarcare' AS Type FROM descarcare";
        return await FetchAndMapAsync(query, ProcessBounds);
    }

    private async IAsyncEnumerable<IDictionary<string, object>> ExportTableAsync(string table, long min, long max, Func<DbDataReader, IDictionary<string, object>> mapRow)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsyncConnection();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {table} WHERE {"data".ToStrftime()} BETWEEN {min.ToStrftime()} AND {max.ToStrftime()} ORDER BY data ASC";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            yield return mapRow(reader);
    }

    private static LastDatesDTO ProcessLastRange(DbDataReader reader)
    {
        var type = "n/a";
        var last = reader["Last"].ConvertTo<long>().ExtractDateTime();
        return new LastDatesDTO(type, last);
    }

    private static LastDatesDTO ProcessLastRanges(DbDataReader reader)
    {
        var type = reader["Type"].ConvertTo<string>();
        var last = reader["Last"].ConvertTo<long>().ExtractDateTime();
        return new LastDatesDTO(type, last);
    }

    private static LongBoundsDTO ProcessBounds(DbDataReader reader)
    {
        var type = reader["Type"].ConvertTo<string>();
        var min = reader["Min"].ConvertTo<long>();
        var max = reader["Max"].ConvertTo<long>();
        return new LongBoundsDTO(type, min, max);
    }
}