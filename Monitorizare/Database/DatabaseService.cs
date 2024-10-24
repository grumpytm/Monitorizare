using Monitorizare.Records;

namespace Monitorizare.Database;

class DatabaseService
{
    private readonly IRecordSaver _recordSaver;

    public DatabaseService()
    {
        _recordSaver = new SqliteRecordsSaver();
    }

    public async Task<(int, int)> ParseAndSave()
    {
        var logParser = new TransportLogsParser();

        (int countIncarcare, int affectedIncarcare) = await ProcessLogAsync(new ProcessIncarcareLogs(), logParser);
        (int countDescarcare, int affectedDescarcare) = await ProcessLogAsync(new ProcessDescarcareLogs(), logParser);

        return (countIncarcare + countDescarcare, affectedIncarcare + affectedDescarcare);
    }

    private async Task<(int count, int affected)> ProcessLogAsync(IProcessTransportLogsFactory factory, TransportLogsParser logParser) =>
        await new ProcessTransportLogs(factory, logParser).ProcessLogFileAsync();
}