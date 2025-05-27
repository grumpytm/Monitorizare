namespace Monitorizare.Forms.Common;

public static class DataGridViewLayout
{
    public static void RenameHeaders(this DataGridView dataGridView, string[] headers)
    {
        var columnCount = dataGridView.ColumnCount;
        for (int i = 0; i < headers.Length && i < columnCount; i++)
            dataGridView.Columns[i].HeaderText = headers[i];
    }

    public static void SetColumnsWidth(this DataGridView dataGridView, int[] widths)
    {
        var columnCount = dataGridView.ColumnCount;
        for (int i = 0; i < widths.Length && i < columnCount; i++)
            dataGridView.Columns[i].Width = widths[i];
    }

    public static void SetSelectionBackColor(this DataGridView dataGridView, Color color) =>
        dataGridView.RowsDefaultCellStyle.SelectionBackColor = color;
}