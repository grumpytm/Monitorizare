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
            MessageBox.Show("Baza de date a fost exportata pe perioada selectata in fisierul 'Export.xls' pe care il gasesti pe Desktop.", "Exportare continut", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            () => _database.LoadIncarcareWithin(incarcare.Min, incarcare.Max),
            () => _database.LoadDescarcareWithin(descarcare.Min, descarcare.Max)
        );
    }

    private async Task<Dictionary<string, object>> GetBoundsFromUI()
    {
        return await PrepareExportAsync
        (
            () => _database.LoadIncarcareWithin(IncarcareBounds.Item1, IncarcareBounds.Item2),
            () => _database.LoadDescarcareWithin(DescarcareBounds.Item1, DescarcareBounds.Item2)
        );
    }

    private static async Task<Dictionary<string, object>> PrepareExportAsync(
        Func<Task<IEnumerable<IncarcareDTO>>> incarcareSource,
        Func<Task<IEnumerable<DescarcareDTO>>> descarcareSource)
    {
        var incarcare = (await incarcareSource()).Select(x => new Dictionary<string, object>
        {
            ["Data"] = x.Date,
            ["Ora"] = x.Time,
            ["Siloz"] = x.Siloz,
            ["Greutate"] = x.Greutate
        });

        var descarcare = (await descarcareSource()).Select(x => new Dictionary<string, object>
        {
            ["Data"] = x.Date,
            ["Ora"] = x.Time,
            ["Siloz"] = x.Siloz,
            ["Greutate"] = x.Greutate,
            ["Hala"] = x.Hala,
            ["Buncar"] = x.Buncar,
        });

        return new Dictionary<string, object> { { "Incarcare", incarcare.ToList() }, { "Descarcare", descarcare.ToList() } };
    }
}