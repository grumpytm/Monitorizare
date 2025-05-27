using System.Data.Common;
using System.Globalization;

namespace Monitorizare.Data.Queries;

public class QueryLogs : QueryDatabase
{
    public async Task<IEnumerable<LogsDTO>> FetchLogs()
    {
        var query = "SELECT type, data, message FROM logs ORDER BY id DESC";
        return await FetchAndMapAsync(query, ProcessLogsData);
    }

    private static LogsDTO ProcessLogsData(DbDataReader reader)
    {
        var type = reader["type"] as string ?? string.Empty;
        var (date, time) = reader["data"].ConvertTo<long>().ExtractDateAndTime();
        var message = reader["message"] as string ?? string.Empty;
        return new LogsDTO(0, type, date, time, message);
    }
}