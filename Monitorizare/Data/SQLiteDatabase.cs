using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Monitorizare.Settings;
using Monitorizare.Records;

namespace Monitorizare.Data;

public class SQLiteDatabase : DatabaseService
{
    protected override DbConnection CreateConnection() =>
        SingletonDB.Instance.CreateConnection();

    private readonly IAppSettings _settings;

    public SQLiteDatabase(IAppSettings settings)
    {
        _settings = settings;
    }

    public override async Task IntegrityCheckAsync()
    {
        try
        {
            var storedQueries = _settings.GetStoredQueries();
            var missingTables = storedQueries.Keys.Except(await GetExistingTables()).ToList();
            if (missingTables.Count == 0) return;

            var queryList = missingTables
                .Select(table => storedQueries.TryGetValue(table, out var query) ? query : string.Empty)
                .Where(NotEmpty);

            await ExecuteTableCreation(queryList);
        }
        catch (Exception ex)
        {
            await LogExceptionAsync(ex);
        }
    }

    private async Task<IEnumerable<string>> GetExistingTables()
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        return connection.GetSchema("Tables")
            .AsEnumerable()
            .Select(x => x[2]?.ToString() ?? string.Empty)
            .Where(NotEmpty);
    }

    private async Task ExecuteTableCreation(IEnumerable<string?> queries)
    {
        using var connection = CreateConnection();

        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            foreach (var query in queries)
            {
                using var command = connection.CreateCommand();
                command.CommandText = query;
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await LogExceptionAsync(ex);
        }
    }

    public override async Task<(int, int)> SaveRecordsAsync(IEnumerable<Queries> queryList)
    {
        int affectedRows = 0;

        using var connection = CreateConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            affectedRows = await ExecuteQueriesAsync(queryList, connection, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await LogExceptionAsync(ex);
            await transaction.RollbackAsync();
        }

        return (queryList.Count(), affectedRows);
    }

    protected override async Task<int> ExecuteQueriesAsync(IEnumerable<Queries> queryList, DbConnection connection, DbTransaction transaction)
    {
        int affectedRows = 0;

        foreach (var item in queryList)
        {
            using var command = connection.CreateCommand();
            command.CommandText = item.Query;
            command.Transaction = transaction;
            affectedRows += await command.ExecuteNonQueryAsync();
        }

        return affectedRows;
    }

    public override async Task LogMessageAsync(string message)
    {
        LogMessage(message);

        DateTime currentTime = DateTime.UtcNow;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

        using var connection = CreateConnection();
        await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO logs (data, msg) VALUES (@dateCreated, @message)";

            var parameters = new Dictionary<string, object>
            {
                { "@message", message },
                { "@dateCreated", unixTime }
            };

            AddParameters(command, parameters);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            LogMessage($"Error: {ex.Message} | Stack trace: {ex.StackTrace}");
        }
    }

    private static void AddParameters(DbCommand command, Dictionary<string, object> parameters)
    {
        foreach (var (key, value) in parameters)
        {
            var param = command.CreateParameter();
            param.ParameterName = key;
            param.Value = value;
            command.Parameters.Add(param);
        }
    }

    private static void LogMessage(string message)
    {
#if DEBUG
        Debug.Print(message);
#endif
    }

    public async Task LogExceptionAsync(Exception ex) => await LogMessageAsync($"Error: {ex.Message} | Stack trace: {ex.StackTrace}");

    private bool NotEmpty(string? line) => !string.IsNullOrEmpty(line);
}