using System.Data.Common;

namespace Monitorizare.Data;

public class TransportRecordSaver
{
    private readonly ILogger _logger;

    private static DbConnection CreateConnection() =>
        SingletonDB.Instance.CreateConnection();

    public TransportRecordSaver() =>
        _logger = LoggerFactory.CreateLogger();

    public async Task<int> SaveRecordsAsync<T>(IEnumerable<T> records) where T : class
    {

        if (!records.Any()) return 0;

        int affectedRows = 0;
        using var connection = CreateConnection();
        await connection.EnsureOpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        using var command = connection.CreateCommand();
        command.Transaction = transaction;

        var (table, columns) = records.First().GetTableAndColumns();
        var values = string.Join(", ", columns.Select(c => $"@{c}"));
        var uniqueKey = columns.First();

        // Prepare SQL statements
        command.CommandText = $"INSERT INTO {table} ({string.Join(", ", columns)}) VALUES ({values}) ON CONFLICT ({uniqueKey}) DO NOTHING";
        command.Prepare();

        try
        {
            foreach (var record in records.Distinct())
                affectedRows += await InsertRecordAsync(command, record);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await _logger.LogExceptionAsync(ex);
        }

        return affectedRows;
    }

    private static async Task<int> InsertRecordAsync<T>(DbCommand command, T record) where T : class
    {
        command.Parameters.Clear();
        var parameters = record.ExtractParameters();
        command.AddParameters(parameters);
        return await command.ExecuteNonQueryAsync();
    }
}