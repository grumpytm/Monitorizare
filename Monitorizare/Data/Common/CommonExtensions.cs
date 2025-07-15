using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Monitorizare.Data.Common;

public static class CommonExtensions
{
    public static long ToUnixTime(this DateTime date) =>
        new DateTimeOffset(date).ToUnixTimeSeconds();

    public static string ToStrftime<T>(this T value) =>
        $"strftime('%Y-%m-%d', datetime({value}, 'unixepoch', 'localtime'))";

    public static T ConvertTo<T>(this object obj) =>
        (T)Convert.ChangeType(obj, typeof(T));

    public static (DateOnly, TimeOnly) ExtractDateAndTime(this long timestamp) =>
        timestamp.ExtractDateTime().ExtractDateAndTime();

    public static (DateOnly, TimeOnly) ExtractDateAndTime(this DateTime dateTime) =>
        (DateOnly.FromDateTime(dateTime), TimeOnly.FromDateTime(dateTime));

    public static DateTime ExtractDateTime(this long timestamp) =>
        DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime.ToLocalTime();

    public static DateOnly ExtractDateOnly(this long timestamp) =>
        DateOnly.FromDateTime(timestamp.ExtractDateTime());

    public static void AddParameter(this IDbCommand command, string name, object value)
    {
        var param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value ?? DBNull.Value;
        command.Parameters.Add(param);
    }

    public static void AddParameters(this IDbCommand command, Dictionary<string, object> parameters)
    {
        foreach (var (name, value) in parameters)
            command.AddParameter(name, value);
    }

    public static async Task<IEnumerable<string>> GetExistingTablesAsync(this DbConnection connection)
    {
        await connection.OpenAsyncConnection();
        return connection.GetSchema("Tables")
            .AsEnumerable()
            .Select(x => x[2]?.ToString() ?? string.Empty)
            .Where(table => table.NotEmpty());
    }

    public static (string, string) GetTableAndColumns<T>(this T record) where T : class =>
        (record.GetType().Name.ToLower(), record.GetRecordColumns());

    public static Dictionary<string, object> ExtractParameters<T>(this T record) where T : class =>
        record.GetProperty(p => new KeyValuePair<string, object>($"@{p.Name}", p.GetValue(record) ?? DBNull.Value))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

    public static IEnumerable<string> GetProperty<T>(this T record, Func<PropertyInfo, string> property) where T : class =>
        record.GetType().GetProperties().Select(property);

    public static IEnumerable<KeyValuePair<string, object>> GetProperty<T>(this T record, Func<PropertyInfo, KeyValuePair<string, object>> property) where T : class =>
        record.GetType().GetProperties().Select(property);

    private static string GetRecordColumns<T>(this T record) where T : class =>
        string.Join(", ", record.GetProperty(p => p.Name.ToLower()));

    public static async Task OpenAsyncConnection(this DbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();
    }
}