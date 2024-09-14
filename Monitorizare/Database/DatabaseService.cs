using System.Data.SQLite;
using System.Diagnostics;

using Monitorizare.Models;
using Monitorizare.Services;

namespace Monitorizare.Database;

public class DatabaseService
{
    public static async Task CheckDatabaseIntegrity()
    {
        var queries = SettingsProvider.GetQueries();

        var expected = queries.Keys.ToList();

        var missing = await DatabaseHelpers.GetMissingTables(expected);

        if (!missing.Any()) return;

        await using var connection = SingletonDB.Instance.GetNewConnection();

        try
        {
            await connection.OpenAsync();

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                await using (var command = new SQLiteCommand(connection))
                {
                    foreach (var table in missing)
                    {
                        command.CommandText = queries.SingleOrDefault(item => item.Key == table).Value;
                        await command.ExecuteNonQueryAsync();

                        command.Parameters.Clear();
                    }
                }

                await transaction.CommitAsync();
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

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                await using (var command = new SQLiteCommand(connection))
                {
                    foreach (var record in records)
                    {
                        var (columns, values, count) = DatabaseHelpers.ExtractColumnsAndValues(record);
                        var tableName = DatabaseHelpers.GetTableNameByColumnCount(count);

                        command.CommandText = $"INSERT OR IGNORE INTO {tableName} ({columns}) VALUES ({values})";
                        affectedRows += await command.ExecuteNonQueryAsync();

                        command.Parameters.Clear();
                    }
                }

                await transaction.CommitAsync();
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