using MetroFramework.Forms;

namespace Monitorizare;

public partial class Migrations : MetroForm
{
    private ILogger Logger { get; } = DatabaseFactory.GetDatabase().Logger;
    private readonly DataGridViewContent _dataGridViewContent = new();
    private IEnumerable<MigrationsDTO> _content = Enumerable.Empty<MigrationsDTO>();

    public Migrations()
    {
        InitializeComponent();

        Shown += (sender, args) => Migrations_FormShown(sender, args);
        FormClosing += (sender, args) => Logs_FormClosing(sender, args);
    }

    // 'ESC' key to close
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData != Keys.Escape)
            return base.ProcessCmdKey(ref msg, keyData);

        Close();
        return true;
    }

    private void Migrations_FormShown(object? sender, EventArgs e)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                _content = await _dataGridViewContent.LoadMigrations();

                DataGridView.Invoke(() =>
                {
                    DataGridView.DataSource = _content.ToList();
                    DataGridView.ClearSelection();

                    var columnsPatterns = new List<(string, int)>()
                    {
                        ("#", 50),
                        ("Nume fisier", 250),
                        ("Data", 80),
                        ("Ora", 70)
                    };

                    DataGridView.AdjustColumnsWith(columnsPatterns); // Adjust the header text and width
                    DataGridView.SetSelectionBackColor(Color.FromArgb(150, 180, 226, 244));
                    DataGridView.Columns[3].DefaultCellStyle.Format = "HH:mm:ss"; // Apply default time format

                    // A quick fix for DataGridView being quirky when comes to aligning stuff
                    DataGridView.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.NotSet;
                    
                    foreach (DataGridViewColumn col in DataGridView.Columns)
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    DataGridView.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                });
            }
            catch (Exception ex)
            {
                await Logger.LogExceptionAsync(ex);
            }
        });
    }

    private void Logs_FormClosing(object? sender, FormClosingEventArgs e)
    {
        var reasons = new List<CloseReason> { CloseReason.WindowsShutDown, CloseReason.ApplicationExitCall, CloseReason.TaskManagerClosing };
        if (!reasons.Contains(e.CloseReason)) FindForm().Dispose();
        DataGridView.DataSource = null; // purge the data source
        _content = Enumerable.Empty<MigrationsDTO>();
    }
}