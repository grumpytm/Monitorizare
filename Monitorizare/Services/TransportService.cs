using System.Diagnostics;
using Monitorizare.Database;

namespace Monitorizare.Services;

class TransportService
{
    private DatabaseService _databaseService;
    
    public TransportService()
    {
        _databaseService = new DatabaseService();
    }

    public async Task<(int, int)> ProcessLogs()
    {
        /* download */
        // await DownloadAsync(); // not implemented yet

        /* parse & save */
        return await _databaseService.ParseAndSave();
    }

    public void DownloadAsync() =>
        throw new NotImplementedException();
}