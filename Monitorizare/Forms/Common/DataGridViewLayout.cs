namespace Monitorizare.Forms.Common;

public static class DataGridViewLayout
{
    public static void RenameHeaders(this DataGridView dataGridView, string[] headers)
    {
        for (int i = 0; i < headers.Length && i < dataGridView.ColumnCount; i++)
            dataGridView.Columns[i].HeaderText = headers[i];
    }

    public static void SetColumnsWidth(this DataGridView dataGridView, int[] widths)
    {
        for (int i = 0; i < widths.Length && i < dataGridView.ColumnCount; i++)
            dataGridView.Columns[i].Width = widths[i];
    }

    public static void AdjustColumnsWith(this DataGridView dataGridView, List<(string, int)> adjustments)
    {
        for (int i = 0; i < adjustments.Count; i++)
        {
            var (header, width) = adjustments[i];
            dataGridView.Columns[i].HeaderText = header; // rename header
            dataGridView.Columns[i].Width = width; // adjust the width
        }
    }

    public static void SetSelectionBackColor(this DataGridView dataGridView, Color color) =>
        dataGridView.RowsDefaultCellStyle.SelectionBackColor = color;
}