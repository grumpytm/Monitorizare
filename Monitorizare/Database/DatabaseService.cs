using System.Data.SQLite;
using Monitorizare.Models;

namespace Monitorizare.Database;

public class DatabaseService
{
    public static async Task<int> SaveTransportRecords(IEnumerable<Transport> records)
    {
        int affectedRows = 0;
        var connection = SingletonDB.GetInstance();
        await connection.OpenAsync();

        using (var transaction = connection.BeginTransaction())
        {
            try
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
            catch (Exception ex)
            {
                transaction.Rollback();
                // Console.WriteLine($"Error: {ex.Message} | Stack trace: {ex.StackTrace}");
                throw;
            }
            finally
            {
                connection.Close();
            }

            return affectedRows;
        }
    }
}