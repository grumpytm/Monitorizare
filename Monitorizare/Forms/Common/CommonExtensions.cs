namespace Monitorizare.Forms.Common;

public static class CommonExtensions
{
    public static void ChangeSizeBasedOn(this GroupBox groupBox, UITabNames tab) =>
        groupBox.Size = UILayoutSizes.GroupBoxSizes.GetValueOrDefault(tab, groupBox.Size);

    public static DataGridViewRow SelectRowBy(this DataGridView dataGridView, Keys keyCode)
    {
        int rowPos = dataGridView.CurrentCell.RowIndex;

        if (keyCode == Keys.Up && rowPos > 0)
            rowPos--;
        else if (keyCode == Keys.Down && rowPos < dataGridView.Rows.Count - 1)
            rowPos++;

        return dataGridView.Rows[rowPos];
    }
}