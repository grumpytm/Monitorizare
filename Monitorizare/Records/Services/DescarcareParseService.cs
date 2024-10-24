using System.Diagnostics;
using Monitorizare.Models;

namespace Monitorizare.Records;

public class DescarcareParseService : IParseService
{
    public string FilePath => "Descarcare.log";
    public IEnumerable<Transport> CreateRecordsFrom(IEnumerable<string> content, TransportLogsParser logsParser)
    {
        foreach (var line in content)
        {
            string[] collection = string.IsNullOrWhiteSpace(line) ? Array.Empty<string>() : line.Split(',');

            if (collection.Length == 0)
                yield break;

            (double timestamp, int siloz, int greutate, int hala, int buncar) = logsParser.ExtractTransportDataFrom(collection);

            yield return new Descarcare(timestamp, siloz, greutate, hala, buncar);
        }
    }
}