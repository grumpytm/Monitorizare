using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

/* 3rd party libs */

using MetroFramework.Forms;
using ComponentFactory.Krypton.Toolkit;
using System.ComponentModel;

namespace Monitorizare
{
    public partial class Vizualizare : MetroForm
    {
        public Vizualizare()
        {
            InitializeComponent();
            this.Icon = Resources.database;

            CurrentTabPage = 0;
            MySQLite.CheckIntegrity();

            /* set min & max on first tab */
            SetMinMax("incarcare");

            ShowTab();
            FixComboStyle();

            TabPage tab = metroTabControl1.SelectedTab;
            DataGridView gridView = (from grid in tab.Controls.OfType<DataGridView>() select grid).FirstOrDefault();

            gridView.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 234, 234, 234);
            gridView.DataBindingComplete += (sender, args) => GridView_DataBindingComplete(sender, args);
        }

        /* Clear default row selection and highlight rows */

        private void GridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs args)
        {
            // Console.WriteLine(String.Format("Status: {0}", isFiltered));

            if (isFiltered)
                return;

            var gridView = ((DataGridView)sender);

            // Console.WriteLine(String.Format("fired for {0}", gridView.Name));

            ((DataGridView)sender).ClearSelection();

            foreach (DataGridViewRow row in ((DataGridView)sender).Rows)
            {
                int value;
                DataTable dt = (gridView.DataSource as DataTable);
                string[] columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();

                /* Highlight rows with wrong value */
                foreach (string column in columnNames.Skip(3))
                {
                    int.TryParse(Convert.ToString(row.Cells[column].Value), out value);
                    if (value == 0)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(150, 252, 179, 188);
                }

                /* Wrong weigth */
                int.TryParse(Convert.ToString(row.Cells["Greutate"].Value), out value);
                if (value < 100)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(150, 252, 179, 188);
            }

            // entry.Key.SelectedIndexChanged += (sender, args) => ComboBox_SelectedIndexChanged(sender, args);
        }

        private int CurrentTabPage { get; set; }
        private bool isFiltered = false;

        /* Form instance */
        public static Vizualizare _vizualizare = null;

        public static Vizualizare Instance
        {
            get
            {
                if (_vizualizare == null)
                {
                    _vizualizare = new Vizualizare();
                }
                return _vizualizare;
            }
        }

        /* ESC to close */

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.ApplicationExitCall || e.CloseReason == CloseReason.TaskManagerClosing)
                return;

            this.FindForm().Dispose();
        }

        private void TabControl1_SelectedIndexchanged(object sender, EventArgs e)
        {
            CurrentTabPage = metroTabControl1.SelectedIndex;
            string table = (CurrentTabPage == 0) ? "incarcare" : "descarcare";
            SetMinMax(table);
            ShowTab();
            FixComboStyle();
        }

        private void SetMinMax(string table)
        {
            List<DateTime> values = MySQLite.SetMinMaxData(table);

            bool isEmpty = !values.Any();

            // Console.WriteLine(String.Format("Values: {0}", String.Join(", ", values)));

            if (isEmpty)
            {
                Console.WriteLine("SQLIte table is empty?");
                Logs.NewRecord("SQLIte table is empty?");
            }
            else
            {
                /* Set min & max dates */
                int skip = (CurrentTabPage == 0) ? 0 : 2;
                List<KryptonDateTimePicker> datePickers = new List<KryptonDateTimePicker>(
                    new KryptonDateTimePicker[] {
                        kryptonDateTimePicker1, kryptonDateTimePicker2, // incarcare
                        kryptonDateTimePicker3, kryptonDateTimePicker4 // descarcare
                    }
                );

                foreach (var ele in datePickers.Skip(skip).Take(2))
                {
                    ele.MinDate = values[0];
                    ele.MaxDate = values[1];
                }
            }
        }

        private void ShowTab()
        {
            /* Create DataTable */
            DataTable dt = new DataTable();
            string dateMin = ((CurrentTabPage < 1) ? kryptonDateTimePicker1 : kryptonDateTimePicker3).Value.Date.ToString("dd.MM.yyyy"); // Incarcare / Descarcare
            string dateMax = ((CurrentTabPage < 1) ? kryptonDateTimePicker2 : kryptonDateTimePicker4).Value.Date.ToString("dd.MM.yyyy");

            int count = (CurrentTabPage < 1) ? 5 : 7;
            string table = (CurrentTabPage < 1) ? "incarcare" : "descarcare";

            var columnsDict = new Dictionary<int, string> {
                { 1, "Timestamp" },
                { 2, "Data" },
                { 3, "Ora" },
                { 4, "Siloz" },
                { 5, "Greutate" },
                { 6, "Hala" },
                { 7, "Buncar" }
            };

            /* Fill GridView with data */
            TabPage tab = metroTabControl1.SelectedTab;
            DataGridView gridView = (from grid in tab.Controls.OfType<DataGridView>() select grid).FirstOrDefault();

            dt = MySQLite.FillTable(dateMin, dateMax, table, columnsDict, count);

            /* Fill ComboBox with values */
            var comboDict = new Dictionary<KryptonComboBox, string> {
                { kryptonComboBox1, "siloz" },
                { kryptonComboBox2, "siloz" },
                { kryptonComboBox3, "hala" },
                { kryptonComboBox4, "buncar" }
            };

            count = (CurrentTabPage == 0) ? 1 : 3;
            int skip = (CurrentTabPage == 0) ? 0 : 1;

            foreach (KeyValuePair<KryptonComboBox, string> entry in comboDict.Skip(skip).Take(count))
            {
                List<Int16> DistinctRows = dt.AsEnumerable().Select(r => r.Field<Int16>(entry.Value)).Distinct().OrderBy(v => v).ToList();

                // Console.WriteLine(String.Format("name: {0}, count: {1}", entry.Value, count));

                entry.Key.Items.Clear();
                if (DistinctRows.Count == 0)
                {
                    entry.Key.Enabled = false;
                }
                else
                {
                    foreach (int item in DistinctRows.OrderBy(v => v))
                        entry.Key.Items.Add(item);

                    entry.Key.Items.Insert(0, "-");
                    entry.Key.SelectedIndex = 0;
                    entry.Key.Enabled = true;
                }

                entry.Key.SelectedIndexChanged += (sender, e) => ComboBox_SelectedIndexChanged(sender, e);
            }

            /* Fill DataGridView */
            gridView.DataSource = dt;

            /*
            DataSet ds = MySQLite.LoadGridData();
            gridView.DataSource = ds.Tables[0].DefaultView;
            */

            /* highlight */
            // gridView.DataBindingComplete -= GridView_DataBindingComplete;

            /* stop multiple binds */
            /*
            gridView.DataBindingComplete += (sender, e) =>
            {
                Console.WriteLine("trick!");

                // gridView.ClearSelection();
                // gridView.DataBindingComplete -= GridView_DataBindingComplete;
            };
            */

            gridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridView.Columns["Data"].DefaultCellStyle.Format = "dd.MM.yyyy";

            /* Adjust columns size */
            List<int> widths = new List<int>(new int[] { 50, 80, 60, 60, 60, 60, 60 });

            for (int x = 0; x < gridView.ColumnCount; x++)
                gridView.Columns[x].Width = widths[x];

            /* Details */
            DisplayDetails();
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage tab = metroTabControl1.SelectedTab;
            DataGridView gridView = (from grid in tab.Controls.OfType<DataGridView>() select grid).FirstOrDefault();

            // string boxName = ((Control)sender).Name;
            // KryptonComboBox box = ((KryptonComboBox)sender);
            // int index = box.SelectedIndex;

            Dictionary<string, int> filters = new Dictionary<string, int>();
            var comboBoxes = tab.Controls.OfType<KryptonComboBox>().OrderBy(x => x.Name).ToList();

            /* test */
            foreach (var ele in comboBoxes)
            {
                if (ele.SelectedIndex > 0)
                {
                    string name = ele.Name.ToString();
                    int.TryParse(ele.SelectedItem.ToString(), out int value);
                    filters.Add(name, value); // box name, value
                }
            }

            string sortBy = string.Empty;
            var dict = new Dictionary<string, string> {
                { "kryptonComboBox1", "siloz" },
                { "kryptonComboBox2", "siloz" },
                { "kryptonComboBox3", "hala" },
                { "kryptonComboBox4", "buncar" }
            };

            foreach (KeyValuePair<string, int> kvp in filters)
            {
                string sort = dict[kvp.Key];
                sortBy = (String.IsNullOrEmpty(sortBy)) ? String.Format("{0} = '{1}'", sort, kvp.Value) : String.Format("{0} AND {1} = '{2}'", sortBy, sort, kvp.Value);
            }

            /* Predict the change and count rows */
            DataTable dt = (gridView.DataSource as DataTable);
            DataView dataView = new DataView(dt);

            dataView.RowFilter = sortBy;
            int count = dataView.Count;

            dataView.Dispose();

            if (count == 0)
                return;

            /* Apply filter */
            dt.DefaultView.RowFilter = (String.IsNullOrEmpty(sortBy)) ? String.Empty : sortBy;

            /* Rebuild index for filtered cells */
            if (gridView.Rows.Count < 1)
                return;

            isFiltered = true;
            int n = 1;
            foreach (DataGridViewRow row in gridView.Rows)
            {
                row.Cells["#"].Value = n;
                n++;
            }
            isFiltered = false;

            /* Other stuff */
            DisplayDetails();
        }

        private void DisplayDetails()
        {
            TabPage tab = metroTabControl1.SelectedTab;
            DataGridView gridView = (from grid in tab.Controls.OfType<DataGridView>() select grid).FirstOrDefault();

            if (gridView.Rows.Count < 1)
                return;

            int total = (gridView.DataSource as DataTable).Rows.Count;
            int count = gridView.Rows.GetRowCount(DataGridViewElementStates.Visible);
            string plural = total == 1 ? "" : "e";

            /* Details */
            var label = (CurrentTabPage < 1) ? kryptonLabel2 : kryptonLabel5;

            label.Visible = true;
            label.Text = String.Format("{0} din {1} rezultat{2} afisate..", count, total, plural);
        }

        public void FixComboStyle()
        {
            TabPage tab = metroTabControl1.SelectedTab;
            var comboBoxes = tab.Controls.OfType<KryptonComboBox>();

            foreach (var ele in comboBoxes)
            {
                ele.DropDownWidth = 60;
                ele.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private void KryptonButton1_Click(object sender, EventArgs e)
        {
            ShowTab();
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            ShowTab();
        }

        private void ExportViewToExcel()
        {
            var dict = new Dictionary<int, string> {
                { 1, "Timestamp" },
                { 2, "Data" },
                { 3, "Ora" },
                { 4, "Siloz" },
                { 5, "Greutate" },
                { 6, "Hala" },
                { 7, "Buncar" }
            };

            string name = (CurrentTabPage < 1) ? "Incarcare" : "Descarcare";

            TabPage tab = metroTabControl1.SelectedTab;
            DataGridView gridView = (from grid in tab.Controls.OfType<DataGridView>() select grid).FirstOrDefault();
            DataTable dt = (gridView.DataSource as DataTable);
            Excel.ExportTable(dt, name);
        }
    }
}