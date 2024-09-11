﻿using System.Data;
using System.Diagnostics;
using Monitorizare.Database;
using Monitorizare.Models;

internal static class DatabaseHelpers
{
    public static (string, string, int) ExtractColumnsAndValues(Transport record)
    {
        var properties = record.GetType().GetProperties()
            .Where(prop => prop.GetValue(record) != null);

        var result = properties
            .Select(prop => new { Name = prop.Name.ToLower(), Value = prop.GetValue(record) })
            .Aggregate((names: "", values: "", count: 0),
                (acc, prop) =>
                (
                    names: acc.names + (acc.count > 0 ? ", " : "") + prop.Name,
                    values: acc.values + (acc.count > 0 ? ", " : "") + $"'{prop.Value}'",
                    count: acc.count + 1
                )
            );

        return (result.names, result.values, result.count);
    }

    public static string GetTableNameByColumnCount(int count)
    {
        var table = count switch
        {
            3 => "incarcare",
            5 => "descarcare",
            _ => HandleUnexpectedCount(count)
        };

        return table;
    }

    private static string HandleUnexpectedCount(int count)
    {
        Debug.Print($"Exception count: {count}");
        throw new NotImplementedException();
    }

    public static async Task<IEnumerable<string?>> GetMissingTables(List<string> expected)
    {
        var instance = SingletonDB.Instance;
        await using var connection = instance.GetNewConnection();
        await connection.OpenAsync();

        var tables = connection.GetSchema("Tables")
            .AsEnumerable()
            .Select(x => x[2]?.ToString() ?? string.Empty)
            .ToList();

        connection.Close();
        return expected.Except(tables);
    }
}