
using Monitorizare.Data;
using Monitorizare.Records;
using Monitorizare.Settings;

namespace Monitorizare.Services;

class TransportService
{
    private readonly IAppSettings _settings;
    private readonly DatabaseService _database;

    private readonly ILogProcessor _logProcessor;

    public TransportService(IAppSettings settings, DatabaseService database, ILogProcessor logProcessor) =>
        (_settings, _database, _logProcessor) = (settings, database, logProcessor);

    public async Task<(int total, int affected)> ParseAndSave()
    {
        var filesList = _settings.GetFilePaths();
        var results = await Task.WhenAll(filesList.Select(async filePath =>
        {
            var records = await _logProcessor.ProcessLogAsync(filePath!);
            return await _database.SaveRecordsAsync(records);
        }));

        return (results.Sum(r => r.Item1), results.Sum(r => r.Item2));
    }
}