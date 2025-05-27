using System.ComponentModel;
using MetroFramework.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Monitorizare;

public partial class Vizualizare : MetroForm
{
    private IEnumerable<TransportContentDTO> _content;
    private UITabNames CurrentTab { get; set; }
    private string CurrentTabName => CurrentTab.ToString().ToLower();
    private bool IsIncarcareTab => CurrentTab == UITabNames.Incarcare;
    private bool IsDescarcareTab => CurrentTab == UITabNames.Descarcare;
    private readonly Dictionary<DataGridView, DataGridViewSorting> _sortHelpers = new();
    private readonly DataGridViewContent _dataGridViewContent = new();

    public Vizualizare()
    {
        InitializeComponent();

        CurrentTab = 0;
        _content = Enumerable.Empty<TransportContentDTO>();

        _ = LoadLastDataAsync();
        _ = SetDateTimeBounds();

        DataGridView.SetSelectionBackColor(Color.FromArgb(150, 180, 226, 244));
        DataGridView.DataBindingComplete += (sender, args) => DataGridView_DataBindingComplete(sender, args);
        FormClosing += (sender, args) => Vizualizare_FormClosing(sender, args);
        Shown += (sender, args) => Vizualizare_FormShown(sender, args);

#if DEBUG
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            Console.WriteLine($"{DateTime.UtcNow} [Unhandled exception] - {args.ExceptionObject}");
        };
#endif
    }

    // 'ESC' key to close
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData != Keys.Escape)
            return base.ProcessCmdKey(ref msg, keyData);

        Close();
        return true;
    }

    private void Vizualizare_FormShown(object? sender, EventArgs e) =>
        PopulateComboBoxes();

    private void Vizualizare_FormClosing(object? sender, FormClosingEventArgs e)
    {
        var reasons = new List<CloseReason> { CloseReason.WindowsShutDown, CloseReason.ApplicationExitCall, CloseReason.TaskManagerClosing };
        if (!reasons.Contains(e.CloseReason)) FindForm().Dispose();
        DataGridView.DataSource = null; // purge the data source
    }

    private async void TabControl_SelectedIndexchanged(object sender, EventArgs e)
    {
        CurrentTab = (UITabNames)TabControl.SelectedIndex;

        await LoadLastDataAsync();
        await SetDateTimeBounds();
        PopulateComboBoxes();
        DataGridView.ResetHeaderCellStyle();
    }

    private async void RefreshButton_Click(object sender, EventArgs e)
    {
        RefreshButton.Enabled = false;
        await LoadDataAsync();
        PopulateComboBoxes();
        DataGridView.ResetHeaderCellStyle();
        RefreshButton.Enabled = true;
    }

    private void DataGridView_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        DataGridView.BeginInvoke(() => DataGridView.ClearSelection());
        DataGridView.HighlightRowsWithIssues();

        if (IsIncarcareTab)
            DataGridView.RemoveUnusedColumns(new[] { "Hala", "Buncar" });
    }

    private async Task SetDateTimeBounds()
    {
        if (!_content.Any()) return;
        var (min, max) = await _dataGridViewContent.GetMinMaxFor(CurrentTabName);

        foreach (var picker in new[] { DateTimePickerMin, DateTimePickerMax })
        {
            picker.MinDate = min;
            picker.MaxDate = max;
            picker.Value = max;
        }
    }

    private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        var grid = (DataGridView)sender;

        if (!_sortHelpers.TryGetValue(grid, out var helper))
            _sortHelpers[grid] = helper = new DataGridViewSorting();

        helper.ColumnHeaderMouseClick(grid, e.ColumnIndex);
    }

    private async Task LoadDataAsync()
    {
        if (string.IsNullOrEmpty(CurrentTabName)) return;

        var (min, max) = DatePickerContent.ParseBounds(DateTimePickerMin, DateTimePickerMax);
        _content = await _dataGridViewContent.LoadData(CurrentTab, min, max);

        LockControls();
        DataGridView.DataSource = _content.ToList();
        DataGridViewCleanup();
    }

    private async Task LoadLastDataAsync()
    {
        _content = await _dataGridViewContent.LoadLastData(CurrentTab);
        DataGridView.DataSource = _content.ToList();
        DataGridViewCleanup();
        LockControls();
    }

    private void LockControls()
    {
        foreach (var control in new Control[] { ComboBoxSiloz, ComboBoxHala, ComboBoxBuncar, DateTimePickerMin, DateTimePickerMax, RefreshButton })
            control.Enabled = _content.Any();
    }

    private void DataGridViewCleanup()
    {
        ResizeCurrentTabControls();
        HideUnusedElements();
        ApplyCustomStyle();
        ShowFilterDetails();
    }

    private void ApplyCustomStyle()
    {
        DataGridView.RenameHeaders(new[] { "#", "Data", "Ora" });
        DataGridView.SetColumnsWidth(new[] { 50, 80, 65, 65, 65, 65, 65 });
    }

    private void ResizeCurrentTabControls()
    {
        FiltrareGroupBox.ChangeSizeBasedOn(CurrentTab);
        DataGridView.ChangeSizeBasedOn(CurrentTab);
    }

    private void HideUnusedElements()
    {
        foreach (var control in new Control[] { LabelHala, LabelBuncar, ComboBoxHala, ComboBoxBuncar })
            control.Visible = IsDescarcareTab;
    }

    private void PopulateComboBoxes()
    {
        var comboBoxList = FiltrareGroupBox.GetVisibleControlsOfType<KryptonComboBox>();
        foreach (var comboBox in comboBoxList)
        {
            comboBox.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
            var distinctItems = _content.ExtractUniqueValuesFor(comboBox);
            comboBox.DataSource = new BindingList<string>(distinctItems.ToList());
            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }
    }

    private void ComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(CurrentTabName)) return;
        if (sender is not KryptonComboBox || !_content.Any()) return;

        var comboBoxList = FiltrareGroupBox.GetVisibleControlsOfType<KryptonComboBox>();
        DataGridView.DataSource = _content.FilterBy(comboBoxList).ToList();

        DataGridView.ResetHeaderCellStyle();
        ShowFilterDetails();
    }

    private void ShowFilterDetails()
    {
        var (count, total) = (DataGridView.RowCount, _content.Count());
        string plural = total == 1 ? "" : "e";

        FilterDetails.Visible = true;
        FilterDetails.Text = $"{count} din {total} rezultat{plural} afisate.";
    }
}