using System.Data.Common;

namespace Monitorizare.Data.Queries;

public abstract class QueryDatabase
{
    private static readonly ILogger _logger = LoggerFactory.CreateLogger();
    private static DbConnection CreateConnection() => SingletonDB.Instance.CreateConnection();

    public static async Task<IEnumerable<T>> FetchAndMapAsync<T>(string query, Func<DbDataReader, T> map)
    {
        var resultList = new List<T>();
        await using var connection = CreateConnection();
        await connection.EnsureOpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = query;
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                resultList.Add(map(reader));
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }

        return resultList;
    }
}