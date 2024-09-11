using System.Data.SQLite;
using System.Diagnostics;
using Monitorizare.Models;

namespace Monitorizare.Database;

public class DatabaseService
{
    public static async Task CheckDatabaseIntegrity()
    {
        var queries = new Dictionary<string, string>
        {
            { "incarcare", "CREATE TABLE `incarcare` (`id` INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, `data` INTEGER NOT NULL UNIQUE, `siloz` INTEGER NOT NULL, `greutate` INTEGER NOT NULL)" },
            { "descarcare", "CREATE TABLE `descarcare`(`id` INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, `data` INTEGER NOT NULL UNIQUE, `siloz` INTEGER NOT NULL, `greutate` INTEGER NOT NULL, `hala` INTEGER NOT NULL, `buncar` INTEGER NOT NULL)" },
            { "logs", "CREATE TABLE `logs` (`id` INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, `data` INTEGER NOT NULL, `msg` TEXT NOT NULL)" }
        };

        var expected = queries.Keys.ToList();

        var missing = await DatabaseHelpers.GetMissingTables(expected);

        if (!missing.Any()) return;

        await using var connection = SingletonDB.Instance.GetNewConnection();

        try
        {
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {
                SQLiteCommand command = connection.CreateCommand();

                foreach (var table in missing)
                {
                    command.CommandText = queries.SingleOrDefault(item => item.Key == table).Value;
                    await command.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message} | Stack trace: {ex.StackTrace}");
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public static async Task<int> SaveTransportRecords(IEnumerable<Transport> records)
    {
        int affectedRows = 0;

        await using var connection = SingletonDB.Instance.GetNewConnection();

        try
        {
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {

                SQLiteCommand command = new SQLiteCommand(connection);

                foreach (var record in records)
                {
                    var (columns, values, count) = DatabaseHelpers.ExtractColumnsAndValues(record);
                    var tableName = DatabaseHelpers.GetTableNameByColumnCount(count);

                    command.CommandText = $"INSERT OR IGNORE INTO {tableName} ({columns}) VALUES ({values})";
                    affectedRows += await command.ExecuteNonQueryAsync();
                    command.Parameters.Clear();
                }

                transaction.Commit();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message} | Stack trace: {ex.StackTrace}");
        }
        finally
        {
            await connection.CloseAsync();
        }

        return affectedRows;
    }
}