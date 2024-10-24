using System.Diagnostics;
using Monitorizare.Database;
using Monitorizare.Models;

namespace Monitorizare.Records;

public class ProcessTransportLogs
{
    private readonly IParseService _parseService;
    private readonly IQueryService _queryService;
    private readonly TransportLogsParser _logsParser;
    private readonly SqliteRecordsSaver _recordSaver;
    private readonly string _filePath;

    public ProcessTransportLogs(IProcessTransportLogsFactory factory, TransportLogsParser logsParser)
    {
        _parseService = factory.CreateParseService();
        _queryService = factory.CreateQueryService();
        _logsParser = logsParser;
        _filePath = _parseService.FilePath;
        _recordSaver = new SqliteRecordsSaver();
    }

    public async Task<(int, int)> ProcessLogFileAsync()
    {
        IEnumerable<string> content = await _logsParser.ParseLogAsync(_filePath);
        IEnumerable<Transport> records = _parseService.CreateRecordsFrom(content, _logsParser);
        List<string> queryList = _queryService.CreateQueryListFrom(records);
        return await _recordSaver.SaveRecordsAsync(queryList);
    }
}