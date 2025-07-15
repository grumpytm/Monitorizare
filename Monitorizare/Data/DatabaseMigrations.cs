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

    public DatabaseMigrations(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

# if DEBUG // Debugging purposes only
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            Console.WriteLine($"{DateTime.UtcNow} [Unhandled exception] - {args.ExceptionObject}");
        };
#endif
    }

    public async Task ApplyMigrationsAsync()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsyncConnection();

        ExistingTables = await connection.GetExistingTablesAsync();
        var lastMigration = string.Empty;

        if (ExistingTables.Contains("migrations"))
            lastMigration = await GetLastAppliedMigrationAsync();

        var pendingMigrations = GetPendingMigrations(lastMigration).OfType<string>();

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

    private async Task<string?> GetLastAppliedMigrationAsync()
    {
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsyncConnection();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT MAX(file) FROM migrations";
            var result = await command.ExecuteScalarAsync();
            return result?.ToString();
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }

        return string.Empty;
    }

    private static IEnumerable<string?> GetPendingMigrations(string? lastMigration)
    {
        var migrationsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "migrations");

        if (!Directory.Exists(migrationsPath))
            return Enumerable.Empty<string>();

        var allFiles = Directory.EnumerateFiles(migrationsPath, "*.sql")
            .Select(Path.GetFileName)
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase);

        return string.IsNullOrEmpty(lastMigration)
            ? allFiles
            : allFiles.SkipWhile(f => string.Compare(f, lastMigration, StringComparison.OrdinalIgnoreCase) <= 0);
    }

    private async Task<(string, bool)> ExecuteMigrationAsync(string file)
    {
        var filePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "migrations"), file);

        try
        {
            var sqlContent = await File.ReadAllTextAsync(filePath);
            var statements = Regex.Split(sqlContent, @"(?<=;)\s*(?=\n|$)")
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("--"));

            await using var connection = CreateConnection();
            await connection.OpenAsyncConnection();

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
                    if (ExistingTables.Contains(tableName))
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

        await using var connection = CreateConnection();
        await connection.OpenAsyncConnection();

        using var command = connection.CreateCommand();
        using var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction;

        try
        {
            var entries = new List<LogEntry>();
            foreach (var fileName in migrations)
            {
                command.CommandText = "INSERT INTO migrations (file, date) VALUES (@file, @date)";
                command.Parameters.Clear();

                var parameters = new Dictionary<string, object>
                {
                    { "@file", fileName},
                    { "@date", DateTime.UtcNow.ToUnixTime() }
                };

                command.AddParameters(parameters);
                await command.ExecuteNonQueryAsync();

                entries.Add(new LogEntry(LogType.Info, $"Migration file `{fileName}` applied successfully."));
            }

            await transaction.CommitAsync();
            await _logger.LogMultipleAsync(entries);
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }
    }
}