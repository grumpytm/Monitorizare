using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Monitorizare.Data;

public class DatabaseMigrations
{
    private readonly ILogger _logger;
    private IEnumerable<string> ExistingTables { get; set; } = Enumerable.Empty<string>();
    private readonly List<LogEntry> MigrationMessages = new();
    private static DbConnection CreateConnection() => SingletonDB.Instance.CreateConnection();

    public DatabaseMigrations(ILogger logger) => _logger = logger;

    public async Task ApplyMigrationsAsync()
    {
        var lastMigration = await FetchLastMigration();
        var pendingMigrations = GetPendingMigrations(lastMigration);
        var successfulMigrations = new List<string>();

        foreach (var file in pendingMigrations)
        {
            var (fileName, success) = await ExecuteMigrationAsync(file);

            if (success)
                successfulMigrations.Add(fileName);
        }

        await _logger.LogMultipleAsync(MigrationMessages);
        await RecordMigrationsAsync(successfulMigrations);
    }

    private async Task<string> FetchLastMigration()
    {
        using var connection = CreateConnection();
        await connection.EnsureOpenAsync();
        ExistingTables = await connection.GetExistingTablesAsync();
        return await QueryIfTableExistsAsync<string>("migrations", "SELECT IFNULL(MAX(file), '') AS Last FROM migrations") ?? string.Empty;
    }

    private static IEnumerable<string> GetPendingMigrations(string lastMigration)
    {
        var settings = new AppSettings();
        var migrationsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settings.MigrationsPath);

        return !Directory.Exists(migrationsPath)
        ? Enumerable.Empty<string>()
        : Directory.EnumerateFiles(migrationsPath, "*.sql")
            .Select(f => Path.GetFileName(f)!) // compiler is paranoid here cos this can’t be null
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .SkipWhile(f => lastMigration.NotEmpty() && StringComparer.OrdinalIgnoreCase.Compare(f, lastMigration) <= 0);
    }

    private async Task<(string, bool)> ExecuteMigrationAsync(string file)
    {
        var filePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "migrations"), file);

        try
        {
            var sqlContent = await File.ReadAllTextAsync(filePath);
            var statements = Regex.Split(sqlContent, @"(?<=;)\s*(?=\n|$)")
                .Select(x => x.Trim())
                .Where(x => x.NotEmpty() && !x.StartsWith("--"));

            using var connection = CreateConnection();
            await connection.EnsureOpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            using var command = connection.CreateCommand();
            command.Transaction = transaction;

            var pattern = @"(?i)^CREATE\s+TABLE\s+[`""]?(?<name>\w+)[`""]?";
            var match = new Regex(pattern);

            foreach (var statement in statements)
            {
                if (match.IsMatch(statement))
                {
                    var tableName = Regex.Match(statement, pattern).Groups["name"].Value;
                    if (ExistingTables.Contains(tableName, StringComparer.OrdinalIgnoreCase))
                    {
                        MigrationMessages.Add(new LogEntry(LogType.Warning, $"Skipped creation of table `{tableName}` from `{file}` migration file cos it already exists."));
                        continue;
                    }
                }

                command.CommandText = statement;
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return (file, true);
        }
        catch (Exception ex)
        {
            MigrationMessages.Add(new LogEntry(LogType.Exception, ex.Message, ex.StackTrace));
            return (file, false);
        }
    }

    private async Task RecordMigrationsAsync(IEnumerable<string> migrations)
    {
        if (!migrations.Any()) return;

        using var connection = CreateConnection();
        await connection.EnsureOpenAsync();

        using var command = connection.CreateCommand();
        using var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction;

        try
        {
            var entryList = new List<LogEntry>();
            command.CommandText = "INSERT INTO migrations (file, date) VALUES (@file, @date)";
            command.Prepare();

            var now = DateTime.UtcNow.ToUnixTime();
            foreach (var fileName in migrations)
            {
                command.Parameters.Clear();

                var parameters = new Dictionary<string, object>
                {
                    { "@file", fileName},
                    { "@date", now }
                };

                command.AddParameters(parameters);
                await command.ExecuteNonQueryAsync();

                entryList.Add(new LogEntry(LogType.Info, $"Migration file `{fileName}` applied successfully."));
            }

            await transaction.CommitAsync();
            await _logger.LogMultipleAsync(entryList);
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }
    }

    private async Task<T?> QueryIfTableExistsAsync<T>(string tableName, string sql)
    {
        using var connection = CreateConnection();
        await connection.EnsureOpenAsync();

        if (!ExistingTables.Contains(tableName, StringComparer.OrdinalIgnoreCase))
            return default;

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            var result = await command.ExecuteScalarAsync();
            return result is DBNull ? default : result!.ConvertTo<T>();
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
            return default;
        }
    }
}