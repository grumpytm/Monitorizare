using System.Data.Common;

namespace Monitorizare.Data;

public class TransportRecordSaver
{
    private readonly ILogger _logger;
    private static DbConnection CreateConnection() =>
        SingletonDB.Instance.CreateConnection();

    public TransportRecordSaver() =>
        _logger = LoggerFactory.CreateLogger();

    public async Task<int> SaveRecordsAsync(IEnumerable<ITransport> records)
    {
        if (!records.Any()) return 0;

        int affectedRows = 0;
        await using var connection = CreateConnection();
        await connection.OpenAsyncConnection();

        using var transaction = await connection.BeginTransactionAsync();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;

        try
        {
            foreach (var record in records)
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

    private static async Task<int> InsertRecordAsync(DbCommand command, ITransport record)
    {
        var (table, columns) = record.GetTableAndColumns();
        var values = record.GetProperty(p => $"@{p.Name}");
        var parameters = record.ExtractParameters();

        command.CommandText = $"INSERT OR IGNORE INTO {table} ({columns}) VALUES ({string.Join(", ", values)})";
        command.AddParameters(parameters);
        return await command.ExecuteNonQueryAsync();
    }
}