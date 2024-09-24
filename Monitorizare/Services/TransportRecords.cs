using Monitorizare.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Monitorizare.Services
{
    public class TransportRecords
    {
        private readonly char[] _trimChars = new char[] { '\t', ',', ' ' };
        private readonly string _pattern = @"^\d{2}/\d{2}/\d{4},\d{2}:\d{2},\d+,\d+(,\d+,\d+)?$";
        private string[] _logs;

        public TransportRecords(string[] logs) =>
            _logs = logs;

        public IEnumerable<Transport> GetTransportRecords() =>
            _logs
                .Where(File.Exists)
                .SelectMany(log => GetRecordsFrom(log))
                .ToList();

        private IEnumerable<Transport> GetRecordsFrom(string log) =>
            GetFilteredContent(log)
                .SelectMany(line => CreateRecord(line));

        public IEnumerable<string> GetFilteredContent(string log) =>
            File.ReadAllLines(log)
                .Select(Sanitize)
                .Where(IsValid);

        private string Sanitize(string line) =>
                line.Replace(";", ",").TrimEnd(_trimChars);

        private bool IsValid(string line) =>
                !string.IsNullOrEmpty(line) && Regex.IsMatch(line, _pattern);

        private IEnumerable<Transport> CreateRecord(string line)
        {
            var parsed = new TransportParse(line);

            if (!parsed.IsValid())
            {
                yield return HandleUnexpectedItemsCount(line);
                yield break;
            }

            int count = parsed.Count;
            var result = parsed.Parse();

            yield return count == 4 ? Incarcare.Create(result.Timestamp, result.Siloz, result.Greutate)
                    : Descarcare.Create(result.Timestamp, result.Siloz, result.Greutate, result.Hala, result.Buncar);
        }

        private static Transport HandleUnexpectedItemsCount(string line)
        {
            // TO DO: record exception with 'line' to database
            var exception = $"Unexpected number of elements while trying to parse this line: {line}";
            Debug.Print($"Exception: {exception}");
            throw new InvalidOperationException(exception);
        }
    }
}
