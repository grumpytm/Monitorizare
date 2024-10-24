using System.Diagnostics;
using Monitorizare.Services;

namespace Monitorizare;

class CronTasks
{
    private readonly Logger _logger;

    private readonly TransportService _transportService;

    public CronTasks()
    {
        _logger = Logger.Instance;
        _transportService = new TransportService();
    }

    public async Task SaveTransportLogs()
    {
        (int recordsCount, int affectedRows) = await _transportService.ProcessLogs();

        string pluralRows = GetPluralOutOf(affectedRows);
        string pluralCount = GetPluralOutOf(recordsCount);

        await _logger.LogAsync($"Transport service report: {recordsCount} valid record{pluralRows} found, {affectedRows} record{pluralRows} where added to database.");
    }

    private string GetPluralOutOf(int count) =>
        count == 1 ? "" : "s";
}