using System.Reflection;

namespace Monitorizare.Forms.Common;

public static class DataGridViewFilter
{
    public static IEnumerable<(string, int)> CreateFilterList<T>(this IEnumerable<T> items, Func<T, string> getName, Func<T, object?> getValue)
    {
        foreach (var item in items)
        {
            var name = getName(item).Replace("ComboBox", string.Empty);
            if (!int.TryParse(getValue(item)?.ToString(), out int expected)) continue;
            yield return (name, expected);
        }
    }

    public static IEnumerable<T> FilterByList<T>(this IEnumerable<T> source, IEnumerable<(string, int)> filters)
    {
        var predicates = new List<Func<T, bool>>();

        foreach (var (propName, expected) in filters)
        {
            var prop = typeof(T).GetProperty(propName);
            if (prop == null) continue;

            predicates.Add(x => prop.GetValue(x) is int value && Equals(value, expected));
        }

        return source.Where(x => predicates.All(predicate => predicate(x)));
    }

    public static IEnumerable<LogsDTO> FilterByType(this IEnumerable<LogsDTO> source, string expected) =>
        source.FilterByOrDefault(x => Equals(x.Type, expected));

    private static IEnumerable<T> FilterByOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var filtered = source.Where(predicate);
        return filtered.Any() ? filtered : source;
    }

    public static IEnumerable<T> ReindexByColumn<T>(this IEnumerable<T> source, string column)
    {
        var prop = typeof(T).GetProperty(column);
        return prop is null ? source : source.ReindexUsing(prop);
    }

    private static IEnumerable<T> ReindexUsing<T>(this IEnumerable<T> source, PropertyInfo prop)
    {
        int index = 1;
        foreach (var item in source)
        {
            prop.SetValue(item, index++);
            yield return item;
        }
    }
}