using System.Data;
using System.Data.SQLite;
using Monitorizare.Services;
using Monitorizare.Settings;

namespace Monitorizare.Database;

public class DatabaseUtils
{
    private readonly SQLiteConnection _connection;
    private readonly Logger _logger;

    public DatabaseUtils()
    {
        _logger = Logger.Instance;
        _connection = SingletonDB.Instance.CreateConnection();
    }

    public async Task DatabaseIntegrityCheck()
    {
        IEnumerable<string> existingTables = await GetCurrentTables();

        Dictionary<string, string?> queries = new SettingsFileParser().GetSettings().Schema;

        List<string> expected = queries.Keys.ToList();

        IEnumerable<string> missingTables = expected.Except(existingTables);

        if (!missingTables.Any()) return;

        List<string> queryList = missingTables.Select(table => queries.SingleOrDefault(item => item.Key == table).Value ?? string.Empty).ToList();

        try
        {
            await _connection.OpenAsync();

            await using (SQLiteTransaction transaction = (SQLiteTransaction)await _connection.BeginTransactionAsync())
            {
                await ExecuteQueriesAsync(queryList, _connection, transaction);

                await transaction.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Error: {ex.Message} | Stack trace: {ex.StackTrace}");
        }
    }

    public async Task<IEnumerable<string>> GetCurrentTables()
    {
        List<string> tables = Enumerable.Empty<string>().ToList();

        try
        {
            using var connection = SingletonDB.Instance.CreateConnection();

            await connection.OpenAsync();

            tables = connection.GetSchema("Tables")
                .AsEnumerable()
                .Select(x => x[2]?.ToString() ?? string.Empty)
                .Where(table => !string.IsNullOrWhiteSpace(table))
                .ToList();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Error: {ex.Message} | Stack trace: {ex.StackTrace}");
        }

        return tables;
    }

    public async Task<int> ExecuteQueriesAsync(List<string> queryList, SQLiteConnection connection, SQLiteTransaction transaction)
    {
        int affectedRows = 0;

        foreach (var query in queryList)
        {
            using SQLiteCommand command = new(query, connection)
            {
                Transaction = transaction
            };

            affectedRows += await command.ExecuteNonQueryAsync();
        }

        return affectedRows;
    }
}