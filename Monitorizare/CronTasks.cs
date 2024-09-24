using System.Data;
using System.Diagnostics;

using Monitorizare.Services;

namespace Monitorizare;

class CronTasks
{
    private TransportService _transportService;

    public CronTasks()
    {
        _transportService = new TransportService();
    }

    public async Task TrySaveTransportLogs()
    {
        var (recordsCount, affectedRows) = await _transportService.ProcessLogs();

        string pluralRows = GetPluralOutOf(affectedRows);
        string pluralCount = GetPluralOutOf(recordsCount);

        Debug.WriteLine($"Transport service report: {recordsCount} valid record{pluralRows} found, {affectedRows} record{pluralRows} where added to database.");
    }

    private string GetPluralOutOf(int count) =>
        count == 1 ? "" : "s";
}