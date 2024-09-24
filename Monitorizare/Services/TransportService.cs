using Monitorizare.Database;

namespace Monitorizare.Services;

public class TransportService
{
    private DatabaseService _databaseService;
    private TransportUpdate _transportUpdate;

    public TransportService()
    {
        _databaseService = new DatabaseService();
        _transportUpdate = new TransportUpdate();
    }

    public async Task<(int, int)> ProcessLogs()
    {
        /* download */
        // await _transportUpdate.DownloadAsync();

        /* parse & save */
        return await _databaseService.ParseAndSave();
    }
}