using MetroFramework.Forms;
using ComponentFactory.Krypton.Toolkit;
using System.ComponentModel;

namespace Monitorizare
{
    public partial class Exporta : MetroForm
    {
        private readonly IDatabase _database;
        private readonly Dictionary<DataGridView, DataGridViewSorting> _sortHelpers = new();
        private readonly DataGridViewContent _dataGridViewContent = new();
        private readonly TransportExportData _transportExport = new();
        private ILogger Logger { get; } = DatabaseFactory.GetDatabase().Logger;

        public Exporta()
        {
            InitializeComponent();

            _database = DatabaseFactory.GetDatabase();
            Shown += (sender, args) => Exporta_FormShown(sender, args);
            FormClosing += (sender, args) => Exporta_FormClosing(sender, args);

            DataGridViewIncarcare.DataBindingComplete += (sender, args) =>
            {
                DataGridViewIncarcare.BeginInvoke(() => DataGridViewIncarcare.ClearSelection());
                DataGridViewIncarcare.HighlightRowsWithIssues();
                DataGridViewIncarcare.RemoveColumns(new List<string> { "Hala", "Buncar" });
                DataGridViewIncarcare.Columns[2].DefaultCellStyle.Format = "HH:mm";
            };

            DataGridViewDescarcare.DataBindingComplete += (sender, args) =>
            {
                DataGridViewDescarcare.BeginInvoke(() => DataGridViewDescarcare.ClearSelection());
                DataGridViewDescarcare.HighlightRowsWithIssues();
                DataGridViewDescarcare.Columns[2].DefaultCellStyle.Format = "HH:mm";
            };
        }

        private void Exporta_FormShown(object? sender, EventArgs e)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await SetDateTimeBounds();
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    await Logger.LogExceptionAsync(ex);
                }
            });

            PopulateExporType();
            ComboBoxExportType.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

            var selectionBackGround = Color.FromArgb(150, 180, 226, 244);

            DataGridViewIncarcare.Invoke(() =>
            {
                DataGridViewIncarcare.SetSelectionBackColor(selectionBackGround);
            });

            DataGridViewDescarcare.Invoke(() =>
            {
                DataGridViewDescarcare.SetSelectionBackColor(selectionBackGround);
            });
        }

        private void Exporta_FormClosing(object? sender, FormClosingEventArgs e)
        {
            var reasons = new List<CloseReason> { CloseReason.WindowsShutDown, CloseReason.ApplicationExitCall, CloseReason.TaskManagerClosing };
            if (!reasons.Contains(e.CloseReason)) FindForm().Dispose();

            DataGridViewIncarcare.DataSource = null; // purge the data source
            DataGridViewDescarcare.DataSource = null;
        }

        private void PopulateExporType()
        {
            ComboBoxExportType.DataSource = new BindingList<string>(new[] { "Ultima zi", "Complet", "Perioada" });
            ComboBoxExportType.SelectedIndex = 0;
            ComboBoxExportType.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /* ESC to close */
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async void ExportButton_Click(object sender, EventArgs e)
        {
            ButtonExport.Enabled = false;
            await ExportData();
            ButtonExport.Enabled = true;
        }

        private void ComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (sender is not KryptonComboBox comboBox) return;
            foreach (var item in new List<Control> { LabelTimeFrameLoading, TimeLoading, LabelTimeFrameUnloading, TimeUnloading })
                item.Visible = comboBox?.SelectedIndex == 2;
        }

        private async Task SetDateTimeBounds()
        {
            var bounds = await _database.GetTransportBounds();
            SetPickerRange(new[] { DateTimePickerLoadingBefore, DateTimePickerLoadingAfter }, bounds.First(x => x.Type == "Incarcare"));
            SetPickerRange(new[] { DateTimePickerUnloadingBefore, DateTimePickerUnloadingAfter }, bounds.First(x => x.Type == "Descarcare"));
        }

        private static void SetPickerRange(IEnumerable<KryptonDateTimePicker> pickers, (long Min, long Max) bounds)
        {
            var (min, max) = (bounds.Min.ExtractDateTime(), bounds.Max.ExtractDateTime());

            foreach (var picker in pickers)
                (picker.MinDate, picker.MaxDate) = (min, max);
        }

        private async Task LoadDataAsync()
        {
            await DataGridViewIncarcare.Invoke(async () =>
            {
                DataGridViewIncarcare.DataSource = (await _dataGridViewContent.LoadLastData(UITabNames.Incarcare)).ToList();
                DataGridViewAdjustColumns(DataGridViewIncarcare);
            });

            await DataGridViewDescarcare.Invoke(async () =>
            {
                DataGridViewDescarcare.DataSource = (await _dataGridViewContent.LoadLastData(UITabNames.Descarcare)).ToList();
                DataGridViewAdjustColumns(DataGridViewDescarcare);
            });
        }

        private static void DataGridViewAdjustColumns(DataGridView dataGrid)
        {
            dataGrid.RenameHeaders(new[] { "#", "Data", "Ora" });
            dataGrid.SetColumnsWidth(new[] { 50, 80, 65, 65, 65, 65, 65 });
        }

        private void DataGridViewIncarcare_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) =>
            DataGridView_ColumnHeaderMouseClick(sender, e);

        private void DataGridViewDescarcare_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) =>
            DataGridView_ColumnHeaderMouseClick(sender, e);

        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            if (!_sortHelpers.TryGetValue(dataGridView, out var helper))
                _sortHelpers[dataGridView] = helper = new DataGridViewSorting();

            helper.ColumnHeaderMouseClick(dataGridView, e.ColumnIndex);
        }

        private async Task ExportData()
        {
            var exportType = ComboBoxExportType.SelectedIndex;

            if (exportType == 2)
            {
                var incarcare = DatePickerContent.ParseBounds(DateTimePickerLoadingBefore, DateTimePickerLoadingAfter);
                var descarcare = DatePickerContent.ParseBounds(DateTimePickerUnloadingBefore, DateTimePickerUnloadingAfter);
                _transportExport.SetBounds(incarcare, descarcare);
            }

            await _transportExport.Export(exportType);
        }
    }
}