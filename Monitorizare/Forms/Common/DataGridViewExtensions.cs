namespace Monitorizare.Forms.Common;

public static class DataGridViewExtensions
{
    public static void HighlightRowsWithIssues(this DataGridView dataGridView)
    {
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (!TryParseValue(row.Cells["Siloz"]).IsWithin(1, 6)) // Incorrect silo number?
                HighlightCell(row.Cells["Siloz"], DataGridViewRowStyle.Color.Error);

            else if (TryParseValue(row.Cells["Greutate"]).IsLessThan(100)) // Weight might be off?
                HighlightCell(row.Cells["Greutate"], DataGridViewRowStyle.Color.Warning);
        }
    }

    private static int TryParseValue(DataGridViewCell cell) =>
        int.TryParse(cell.Value.ToString(), out int value) ? value : 0;

    private static void HighlightCell(DataGridViewCell cell, Color backgroundColor)
    {
        cell.Style.Font = DataGridViewRowStyle.Font.Bold;
        cell.OwningRow.DefaultCellStyle.BackColor = backgroundColor;
    }

    public static void RemoveUnusedColumns(this DataGridView datagridview, string[] collection)
    {
        foreach (var name in collection)
            datagridview.Columns.Remove(name);
    }

    public static void ChangeSizeBasedOn(this DataGridView dataGridView, UITabNames tab) =>
        dataGridView.Size = UILayoutSizes.DataGridViewSizes.GetValueOrDefault(tab, dataGridView.Size);

    public static void ResetHeaderCellStyle(this DataGridView dataGridView)
    {
        for (int i = 0; i < dataGridView.ColumnCount; i++)
            dataGridView.Columns[i].HeaderCell.Style.Font = DataGridViewRowStyle.Font.Regular;
    }
}