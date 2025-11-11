using System.Data.Common;

namespace Monitorizare.Data.Queries;

public class QueryMigrations : QueryDatabase
{
    public async Task<IEnumerable<MigrationsDTO>> FetchMigrationsAsync()
    {
        var query = "SELECT * FROM migrations ORDER BY id ASC";
        return await FetchAndMapAsync(query, ProcessData);
    }

    private static MigrationsDTO ProcessData(DbDataReader reader)
    {
        var file = reader["file"] as string ?? string.Empty;
        var (date, time) = reader["timestamp"].ConvertTo<long>().ExtractDateAndTime();
        return new MigrationsDTO(0, file, date, time);
    }
}