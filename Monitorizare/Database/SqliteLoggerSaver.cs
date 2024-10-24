using System.Data.SQLite;
using System.Diagnostics;

namespace Monitorizare.Database;

public class SqliteLoggerSaver
{
    private readonly SQLiteConnection _connection;
    public SqliteLoggerSaver()
    {
        _connection = SingletonDB.Instance.CreateConnection();
    }

    public async Task SaveLog(string message)
    {
        DateTime currentTime = DateTime.UtcNow;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

        // Perform database integrity check
        await new DatabaseUtils().DatabaseIntegrityCheck();

        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "INSERT INTO logs (data, msg) VALUES (@dateCreated, @message)";
            command.Parameters.AddWithValue("@message", message);
            command.Parameters.AddWithValue("@dateCreated", unixTime);

            await _connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message} | Stack trace: {ex.StackTrace}");
        }
    }
}