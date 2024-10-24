using Monitorizare.Services;
using System.Data.SQLite;

namespace Monitorizare.Database;

public class SqliteRecordsSaver : IRecordSaver
{
    private readonly Logger _logger;
    private readonly SQLiteConnection _connection;
    private readonly DatabaseUtils _databaseUtils;

    public SqliteRecordsSaver()
    {
        _logger = Logger.Instance;
        _connection = SingletonDB.Instance.CreateConnection();
        _databaseUtils = new DatabaseUtils();
    }

    public async Task<(int, int)> SaveRecordsAsync(List<string> queryList)
    {
        int affectedRows = 0;
        try
        {
            // Perform a database integrity check
            await _databaseUtils.DatabaseIntegrityCheck();

            await _connection.OpenAsync();
            await using (SQLiteTransaction transaction = (SQLiteTransaction)await _connection.BeginTransactionAsync())
            {
                affectedRows += await _databaseUtils.ExecuteQueriesAsync(queryList, _connection, transaction);
                await transaction.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Error: {ex.Message} | Stack trace: {ex.StackTrace}");
        }

        return (queryList.Count(), affectedRows);
    }
}