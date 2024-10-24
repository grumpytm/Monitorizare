using Monitorizare.Models;

namespace Monitorizare.Records;

public interface IParseService
{
    public string FilePath { get; }

    public IEnumerable<Transport> CreateRecordsFrom(IEnumerable<string> content, TransportLogsParser logsParser);
}