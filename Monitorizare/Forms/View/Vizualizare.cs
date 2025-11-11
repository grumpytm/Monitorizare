using System.ComponentModel;
using MetroFramework.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Monitorizare;

public partial class Vizualizare : MetroForm
{
    private IEnumerable<TransportContentDTO> _content = Enumerable.Empty<TransportContentDTO>();
    private UITabNames CurrentTab { get; set; }
    private string CurrentTabName => CurrentTab.ToString().ToLower();
    private bool IsIncarcareTab => CurrentTab == UITabNames.Incarcare;
    private bool IsDescarcareTab => CurrentTab == UITabNames.Descarcare;
    private readonly Dictionary<DataGridView, DataGridViewSorting> _sortHelpers = new();
    private readonly DataGridViewContent _dataGridViewContent = new();
    private ILogger Logger { get; } = DatabaseFactory.GetDatabase().Logger;

    public Vizualizare()
    {
        InitializeComponent();

        CurrentTab = UITabNames.Incarcare;
        Shown += (sender, args) => Vizualizare_FormShown(sender, args);
        FormClosing += (sender, args) => Vizualizare_FormClosing(sender, args);

        _ = Task.Run(async () =>
        {
            try
            {
                await LoadLastDataAsync();
                await SetDateTimeBounds();
            }
            catch (Exception ex)
            {
                await Logger.LogExceptionAsync(ex);
            }
        });

        DataGridView.DataBindingComplete += (sender, args) =>
        {
            DataGridView.BeginInvoke(() => DataGridView.ClearSelection());
            DataGridView.HighlightRowsWithIssues();

            if (IsIncarcareTab)
                DataGridView.RemoveColumns(new List<string> { "Hala", "Buncar" });

            DataGridView.Columns[2].DefaultCellStyle.Format = "HH:mm"; // Apply default time format
        };
    }

    // 'ESC' key to close
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData != Keys.Escape)
            return base.ProcessCmdKey(ref msg, keyData);

        Close();
        return true;
    }

    private void Vizualizare_FormShown(object? sender, EventArgs e)
    {
        PopulateComboBoxes();
        DataGridView.SetSelectionBackColor(Color.FromArgb(150, 180, 226, 244));
    }

    private void Vizualizare_FormClosing(object? sender, FormClosingEventArgs e)
    {
        var reasons = new List<CloseReason> { CloseReason.WindowsShutDown, CloseReason.ApplicationExitCall, CloseReason.TaskManagerClosing };
        if (!reasons.Contains(e.CloseReason)) FindForm().Dispose();
        DataGridView.DataSource = null; // purge the data source
        _content = Enumerable.Empty<TransportContentDTO>();
    }

    private async void TabControl_SelectedIndexchanged(object sender, EventArgs e)
    {
        SetCurrentTab(TabControl.SelectedIndex);

        await LoadLastDataAsync();
        await SetDateTimeBounds();
        
        PopulateComboBoxes();
        DataGridView.ResetHeaderCellStyle();
    }

    private void SetCurrentTab(int value) =>
        CurrentTab = Enum.IsDefined(typeof(UITabNames), value)
        ? (UITabNames)value
        : 0;

    private async void RefreshButton_Click(object sender, EventArgs e)
    {
        RefreshButton.Enabled = false;
        await LoadDataAsync();
        PopulateComboBoxes();
        DataGridView.ResetHeaderCellStyle();
        RefreshButton.Enabled = true;
    }

    private async Task SetDateTimeBounds()
    {
        if (!_content.Any()) return;
        var (min, max) = await _dataGridViewContent.GetMinMaxFor(CurrentTabName);

        foreach (var picker in new[] { DateTimePickerMin, DateTimePickerMax })
        {
            picker.Invoke(() =>
            {
                picker.MinDate = min;
                picker.MaxDate = max;
                picker.Value = max;
            });
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
        _content = await _dataGridViewContent.LoadData(CurrentTab, new DateBounds(min, max));
        PopulateDataGridView();
    }

    private async Task LoadLastDataAsync()
    {
        _content = await _dataGridViewContent.LoadLastData(CurrentTab);
        PopulateDataGridView();
    }

    private void PopulateDataGridView()
    {
        DataGridView.Invoke(() => DataGridView.DataSource = _content.ToList());

        DataGridViewCleanup();
        LockControls();
    }

    private void DataGridViewCleanup()
    {
        ResizeCurrentTabControls();
        HideUnusedElements();
        ApplyCustomStyle();
        ShowFilterDetails();
    }

    private void LockControls()
    {
        var count = _content.Any();
        foreach (var control in new Control[] { ComboBoxSiloz, ComboBoxHala, ComboBoxBuncar, DateTimePickerMin, DateTimePickerMax, RefreshButton })
            control.Invoke(() => control.Enabled = count);
    }

    private void ApplyCustomStyle()
    {
        DataGridView.Invoke(() =>
        {
            DataGridView.RenameHeaders(new[] { "#", "Data", "Ora" });
            DataGridView.SetColumnsWidth(new[] { 50, 80, 65, 65, 65, 65, 65 });
        });
    }

    private void ResizeCurrentTabControls()
    {
        FiltrareGroupBox.Invoke(() => FiltrareGroupBox.ChangeSizeBasedOn(CurrentTab));
        DataGridView.Invoke(() => DataGridView.ChangeSizeBasedOn(CurrentTab));
    }

    private void HideUnusedElements()
    {
        foreach (var control in new Control[] { LabelHala, LabelBuncar, ComboBoxHala, ComboBoxBuncar })
            control.Invoke(() => control.Visible = IsDescarcareTab);
    }

    private void PopulateComboBoxes()
    {
        var comboBoxList = FiltrareGroupBox.GetVisibleControlsOfType<KryptonComboBox>();

        foreach (var comboBox in comboBoxList)
        {
            var propName = comboBox.Name.Replace("ComboBox", "");

            var availableList = new List<string> { "-" };
            availableList.AddRange(_content.GetDistinctOrderedPropertyValues(propName));

            comboBox.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
            comboBox.DataSource = new BindingList<string>(availableList);
            comboBox.SelectedIndex = 0;
            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }
    }

    private void ComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(CurrentTabName)) return;
        if (sender is not KryptonComboBox || !_content.Any()) return;

        var availableList = FiltrareGroupBox.GetVisibleControlsOfType<KryptonComboBox>();
        var comboBoxList = availableList.CreateFilterList(x => x.Name, x => x.SelectedValue);

        DataGridView.DataSource = _content.FilterByList(comboBoxList).ReindexByColumn("Id").ToList();
        DataGridView.ResetHeaderCellStyle();
        ShowFilterDetails();
    }

    private void ShowFilterDetails()
    {
        var (count, total) = (DataGridView.RowCount, _content.Count());
        string plural = total == 1 ? "" : "e";

        FilterDetails.Invoke(() =>
        {
            FilterDetails.Visible = true;
            FilterDetails.Text = $"{count} din {total} rezultat{plural} afisate.";
        });
    }
}