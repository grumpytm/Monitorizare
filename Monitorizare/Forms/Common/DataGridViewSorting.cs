namespace Monitorizare.Forms.Common;

public class DataGridViewSorting
{
    private int _lastUsedIndex;
    private bool _sortDirection;

    public void ColumnHeaderMouseClick(DataGridView dataGridView, int columnIndex)
    {
        string column = GetColumnToSortBy(dataGridView, columnIndex);

        // sort column
        var content = dataGridView.DataSource as IEnumerable<TransportContentDTO> ?? Enumerable.Empty<TransportContentDTO>();
        dataGridView.DataSource = SortContentBy(content, column).ToList();

        // highlight column
        for (int i = 0; i < dataGridView.ColumnCount; i++)
            dataGridView.Columns[i].HeaderCell.Style.Font = DataGridViewRowStyle.Font.Regular;

        dataGridView.Columns[column].HeaderCell.Style.Font = DataGridViewRowStyle.Font.Bold;
    }

    private string GetColumnToSortBy(DataGridView dataGridView, int index)
    {
        if (_lastUsedIndex == index)
            _sortDirection ^= true; // toggle direction
        else
            _sortDirection = true; // default set to 'ascending' when switching to another column

        _lastUsedIndex = index;

        return dataGridView.Columns[index].Name;
    }

    private IEnumerable<TransportContentDTO> SortContentBy(IEnumerable<TransportContentDTO> content, string column) => _sortDirection
        ? content.OrderBy(item => item.GetType().GetProperty(column)?.GetValue(item) ?? string.Empty)
        : content.OrderByDescending(item => item.GetType().GetProperty(column)?.GetValue(item) ?? string.Empty);
}