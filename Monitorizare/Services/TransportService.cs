using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

using Monitorizare.Database;
using Monitorizare.Models;

namespace Monitorizare.Services;

public class TransportService
{

    public static async Task<(int, int)> SaveLogsToDatabaseAsync(string[] logFiles)
    {
        var allRecords = GenerateRecordsFromLogs(logFiles);
        int affectedRows = await DatabaseService.SaveTransportRecords(allRecords);
        int recordsCount = allRecords.Count();

        return (affectedRows, recordsCount);
    }

    private static IEnumerable<Transport> GenerateRecordsFromLogs(string[] files)
    {
        var records = new List<Transport>();

        foreach (var file in files)
        {
            var record = ParseLogFile(file);
            records.AddRange(record);
        }

        return records;
    }

    private static IEnumerable<Transport> ParseLogFile(string logFile)
    {
        char[] trim = new char[] { '\t', ',', ' ' };
        string pattern = @"^\d{2}/\d{2}/\d{4},\d{2}:\d{2},\d+,\d+(,\d+,\d+)?$";

        var fileContent = File.ReadAllLines(logFile)
            .Select(x => x.Replace(";", ",").TrimEnd(trim))
            .Where(line => (!line.Equals(string.Empty) && Regex.IsMatch(line, pattern)));

        foreach (var line in fileContent)
        {
            string[] items = line.Split(',');
            int count = items.Length;

            DateOnly dateOnly = DateOnly.ParseExact(items[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            TimeOnly timeOnly = TimeOnly.ParseExact(items[1], "H:mm", CultureInfo.InvariantCulture);

            var timestamp = Math.Truncate(dateOnly.ToDateTime(timeOnly)
                .ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1))
                .TotalSeconds);

            int.TryParse(items[2], out var siloz);
            int.TryParse(items[3], out var greutate);
            int.TryParse(count == 6 ? items[4] : string.Empty, out var hala);
            int.TryParse(count == 6 ? items[5] : string.Empty, out var buncar);

            yield return count switch
            {
                4 => Transport.Incarcare(timestamp, siloz, greutate),
                6 => Transport.Descarcare(timestamp, siloz, greutate, hala, buncar),
                _ => HandleUnexpectedElements(line)
            };
        }
    }

    private static Transport HandleUnexpectedElements(string line)
    {
        // TO DO: record exception with 'line' to database
        var exception = new InvalidOperationException("Unexpected number of elements while trying to parse this line: {line}");
        Debug.Print($"Exception: {exception}");
        throw exception;
    }
}
