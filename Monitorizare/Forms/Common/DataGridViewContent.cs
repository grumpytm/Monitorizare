namespace Monitorizare.Forms.Common;

public class DataGridViewContent
{
    private readonly IDatabase _database;

    public DataGridViewContent()
    {
        _database = DatabaseFactory.GetDatabase();
    }

    public async Task<IEnumerable<TransportContentDTO>> LoadData(UITabNames tab, long min, long max)
    {
        return tab switch
        {
            UITabNames.Incarcare => (await _database.LoadIncarcareWithin(min, max))
                .Select((item, index) => new TransportContentDTO(index + 1, item.Date, item.Time, item.Siloz, item.Greutate, 0, 0)),
            UITabNames.Descarcare => (await _database.LoadDescarcareWithin(min, max))
                .Select((item, index) => new TransportContentDTO(index + 1, item.Date, item.Time, item.Siloz, item.Greutate, item.Hala, item.Buncar)),
            _ => Enumerable.Empty<TransportContentDTO>()
        };
    }

    public async Task<IEnumerable<TransportContentDTO>> LoadLastData(UITabNames tab)
    {
        return tab switch
        {
            UITabNames.Incarcare => (await _database.LastIncarcareRecords())
                .Select((item, index) => new TransportContentDTO(index + 1, item.Date, item.Time, item.Siloz, item.Greutate, 0, 0)),
            UITabNames.Descarcare => (await _database.LastDescarcareRecords())
                .Select((item, index) => new TransportContentDTO(index + 1, item.Date, item.Time, item.Siloz, item.Greutate, item.Hala, item.Buncar)),
            _ => Enumerable.Empty<TransportContentDTO>()
        };
    }

    public async Task<DateTimeBoundsDTO> GetMinMaxFor(string tabName) =>
        (await _database.GetMinMaxFor(tabName)).First();

    public async Task<IEnumerable<LogsDTO>> LoadLogs() =>
        (await _database.FetchLogsAsync()).Select((item, index) => new LogsDTO(index + 1, item.Type, item.Date, item.Time, item.Message));
}