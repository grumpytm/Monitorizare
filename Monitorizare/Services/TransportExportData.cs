using MiniExcelLibs;

namespace Monitorizare.Services;

public class TransportExportData
{
    private readonly ILogger _logger;
    private readonly IDatabase _database;
    private (long, long) IncarcareBounds { get; set; }
    private (long, long) DescarcareBounds { get; set; }

    public TransportExportData()
    {
        _logger = LoggerFactory.CreateLogger();
        _database = DatabaseFactory.GetDatabase();
    }

    delegate Task<IEnumerable<IncarcareDTO>> IncarcareSource();
    delegate Task<IEnumerable<DescarcareDTO>> DescarcareSource();

    public void SetBounds((long, long) incarcare, (long, long) descarcare)
    {
        IncarcareBounds = incarcare;
        DescarcareBounds = descarcare;
    }

    public async Task Export(int exportType)
    {
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Export.xlsx");

        try
        {
            var sheets = exportType switch
            {
                0 => await PrepareExportAsync(_database.LastIncarcareRecords, _database.LastDescarcareRecords),
                1 => await GetBoundsFromDB(),
                2 => await GetBoundsFromUI(),
                _ => new()
            };

            MiniExcel.SaveAs(filePath, sheets, overwriteFile: true);
            sheets.Clear();

            var message = "Baza de date a fost exportata pe perioada selectata in fisierul 'Export.xls' pe care il gasesti pe Desktop.";
            MessageBox.Show(message, "Exportare continut", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            await _logger.LogExceptionAsync(ex);
        }
    }

    private async Task<Dictionary<string, object>> GetBoundsFromDB()
    {
        var bounds = await _database.GetTransportBounds();
        var incarcare = bounds.First(x => x.Type == "Incarcare");
        var descarcare = bounds.First(x => x.Type == "Descarcare");

        return await PrepareExportAsync
        (
            () => _database.LoadIncarcareWithin(new DateBounds(incarcare.Min, incarcare.Max)),
            () => _database.LoadDescarcareWithin(new DateBounds(descarcare.Min, descarcare.Max))
        );
    }

    private async Task<Dictionary<string, object>> GetBoundsFromUI()
    {
        return await PrepareExportAsync
        (
            () => _database.LoadIncarcareWithin(new DateBounds(IncarcareBounds.Item1, IncarcareBounds.Item2)),
            () => _database.LoadDescarcareWithin(new DateBounds(DescarcareBounds.Item1, DescarcareBounds.Item2))
        );
    }

    private static async Task<Dictionary<string, object>> PrepareExportAsync(IncarcareSource incarcareSource, DescarcareSource descarcareSource) => new()
    {
        { "Incarcare", (await incarcareSource()).Select(ToDictionary).ToList() },
        { "Descarcare", (await descarcareSource()).Select(ToDictionary).ToList() }
    };

    private static Dictionary<string, object?> ToDictionary<T>(T dto) =>
        typeof(T).GetProperties()
            .ToDictionary(prop => ColumnNames.TryGetValue(prop.Name, out var newName) ? newName : prop.Name, prop => prop.GetValue(dto));

    private static readonly Dictionary<string, string> ColumnNames = new()
    {
        ["Time"] = "Ora",
        ["Date"] = "Data"
    };
}