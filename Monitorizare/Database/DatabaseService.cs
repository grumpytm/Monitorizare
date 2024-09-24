using System.Data.SQLite;
using System.Diagnostics;

using Monitorizare.Models;
using Monitorizare.Services;

namespace Monitorizare.Database;

public class DatabaseService
{
    private string[] _logs;
    private TransportRecords _records;
    private DatabaseHelpers _databaseHelpers;
    private SettingsProvider _settingsProvider;

    public DatabaseService()
    {
        _settingsProvider = new SettingsProvider();
        _logs = _settingsProvider.GetLogFiles();
        _records = new TransportRecords(_logs);
        _databaseHelpers = new DatabaseHelpers();
    }

    public async Task CheckIntegrity()
    {
        var queries = _settingsProvider.GetQueries();
        var expected = queries.Keys.ToList();
        var missing = await _databaseHelpers.GetMissingTables(expected);

        if (!missing.Any()) return;

        var queryList = missing
                .Select(table => queries.SingleOrDefault(item => item.Key == table).Value ?? string.Empty)
                .ToList();

        await DatabaseSave(queryList);
    }

    public async Task<(int, int)> ParseAndSave()
    {
        var records = _records.GetTransportRecords();

        int affectedRows = 0;
        int count = records.Count();

        var queryList = GenerateInsertQueries(records);

        affectedRows += await DatabaseSave(queryList);

        return (count, affectedRows);
    }

    private List<string> GenerateInsertQueries(IEnumerable<Transport> records) =>
        records
            .Select(record => record switch
            {
                Incarcare i => $"INSERT OR IGNORE INTO incarcare (data, siloz, greutate) VALUES ('{i.data}', '{i.siloz}', '{i.greutate}')",
                Descarcare d => $"INSERT OR IGNORE INTO descarcare(data, siloz, greutate, hala, buncar) VALUES ('{d.data}', '{d.siloz}', '{d.greutate}', '{d.hala}', '{d.buncar}')",
                _ => string.Empty
            })
            .Where(query => !string.IsNullOrEmpty(query))
            .ToList();

    public async Task<int> DatabaseSave(List<string> queryList)
    {
        int affectedRows = 0;
        
        await using var connection = SingletonDB.Instance.CreateConnection();

        try
        {
            await connection.OpenAsync();

            await using (var transaction = await connection.BeginTransactionAsync())
            {
                await using (var command = new SQLiteCommand(connection))
                {

                    var tasks = queryList.Select(query =>
                    {
                        command.CommandText = query;
                        return command.ExecuteNonQueryAsync();
                    });

                    affectedRows += (await Task.WhenAll(tasks)).Sum();
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