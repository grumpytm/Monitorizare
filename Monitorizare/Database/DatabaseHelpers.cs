using System.Data;
using Monitorizare.Database;

public class DatabaseHelpers
{
    public async Task<IEnumerable<string>> GetMissingTables(List<string> expected)
    {
        await using var connection = SingletonDB.Instance.CreateConnection();
        await connection.OpenAsync();

        var tables = connection.GetSchema("Tables")
            .AsEnumerable()
            .Select(x => x[2]?.ToString() ?? string.Empty)
            .Where(table => !string.IsNullOrWhiteSpace(table))
            .ToList();

        return expected.Except(tables);
    }
}
