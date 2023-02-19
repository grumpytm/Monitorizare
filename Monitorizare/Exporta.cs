using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;

/* 3rd party libs */

using MetroFramework.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace Monitorizare
{
    public partial class Exporta : MetroForm
    {
        public Exporta()
        {
            InitializeComponent();
            this.Icon = Resources.database;

            kryptonDataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 234, 234, 234);
            kryptonDataGridView2.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 234, 234, 234);

            InitializeDataGridView();
            SanityCheck();
        }

        /* Form instance */
        public static Exporta _exporta = null;

        public static Exporta Instance
        {
            get
            {
                if (_exporta == null)
                {
                    _exporta = new Exporta();
                }
                return _exporta;
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

        /* Clear default row selection */

        private void datagridview_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ((DataGridView)sender).ClearSelection();
            DataTable dt = (((DataGridView)sender).DataSource as DataTable);
            string[] columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();

            try
            {
                foreach (DataGridViewRow row in ((DataGridView)sender).Rows)
                {
                    int value;

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
            }
            catch (StackOverflowException ex)
            {
                Console.WriteLine(String.Format("Exception: {0}\r\n | Stack trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        private void AdjustAndHighlight()
        {
            var gridViews = this.Controls.OfType<KryptonDataGridView>().OrderBy(x => x.Name).ToList();
            foreach (var gridView in gridViews)
            {
                gridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridView.Columns["Data"].DefaultCellStyle.Format = "dd.MM.yyyy";

                /* Adjust columns size */
                List<int> widths = new List<int>(new int[] { 50, 80, 60, 60, 60, 60, 60 });

                int count = gridView.ColumnCount;

                for (int x = 0; x < count; x++)
                    gridView.Columns[x].Width = widths[x];

                if (count == 0)
                    return;
            }
        }

        private void SanityCheck()
        {
            int count = 0;
            var gridViews = this.Controls.OfType<KryptonDataGridView>().OrderBy(x => x.Name).ToList();
            foreach (var gridView in gridViews)
            {
                if (gridView.Rows.Count > 0)
                    count++;
                else
                    gridView.Columns[0].HeaderText = "#";
            }

            if (count == 0)
                metroPanel1.Visible = false;

            if (count > 0)
            {
                kryptonButton1.Enabled = true;
                metroPanel1.Visible = true;
            }
        }

        private void InitializeDataGridView()
        {
            kryptonDataGridView1.DataSource = MySQLite.LoadLastRows("incarcare");
            kryptonDataGridView2.DataSource = MySQLite.LoadLastRows("descarcare");
            AdjustAndHighlight();
        }

        private void SetMinMax()
        {
            List<DateTime> values = MySQLite.SetMinMaxDataMultiple();

            bool isEmpty = !values.Any();

            if (isEmpty)
            {
                Console.WriteLine("empty");
            }
            else
            {
                /* Set min & max dates */
                kryptonDateTimePicker1.MinDate = values[0];
                kryptonDateTimePicker1.MaxDate = values[1];

                kryptonDateTimePicker2.MinDate = values[0];
                kryptonDateTimePicker2.MaxDate = values[1];

                kryptonDateTimePicker3.MinDate = values[2];
                kryptonDateTimePicker3.MaxDate = values[3];

                kryptonDateTimePicker4.MinDate = values[2];
                kryptonDateTimePicker4.MaxDate = values[3];
            }
        }

        private void metroRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            metroPanel2.Visible = false;

            SanityCheck();
        }

        private void metroRadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            SetMinMax();
            metroPanel2.Visible = true;
            SanityCheck();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            var excelFile = String.Format("{0}/Export.xls", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            bool locked = Functions.IsFileLocked(excelFile);

            if (locked)
            {
                MessageBox.Show("Te rog sa inchizi fisierul Export.xls pentru a putea fi inlocuit.", "Eroare: fisier deschis!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dict = new Dictionary<int, string> {
                { 1, "Timestamp" },
                { 2, "Data" },
                { 3, "Ora" },
                { 4, "Siloz" },
                { 5, "Greutate" },
                { 6, "Hala" },
                { 7, "Buncar" }
            };

            try
            {
                bool full = metroRadioButton1.Checked;
                bool time = metroRadioButton2.Checked;

                if (full)
                {
                    // everything
                    DataTable incarcare = MySQLite.FetchTable("incarcare", dict, 5);
                    DataTable descarcare = MySQLite.FetchTable("descarcare", dict, 7);

                    Excel.WriteSheets(incarcare, descarcare);

                    // Console.WriteLine(String.Format("Incarcare: {0} | Descarcare: {1}", incarcare.Rows.Count, descarcare.Rows.Count));
                }
                else if (time)
                {
                    // time frame
                    string a = kryptonDateTimePicker1.Value.Date.ToString("dd.MM.yyyy");
                    string b = kryptonDateTimePicker2.Value.Date.ToString("dd.MM.yyyy");
                    string c = kryptonDateTimePicker3.Value.Date.ToString("dd.MM.yyyy");
                    string d = kryptonDateTimePicker4.Value.Date.ToString("dd.MM.yyyy");

                    DataTable incarcare = MySQLite.FetchTable(a, b, "incarcare", dict, 5);
                    DataTable descarcare = MySQLite.FetchTable(c, d, "descarcare", dict, 7);

                    Excel.WriteSheets(incarcare, descarcare);

                    // Console.WriteLine(String.Format("Incarcare: {0} | Descarcare: {1}", incarcare.Rows.Count, descarcare.Rows.Count));
                }
            }
            catch { }
            finally
            {
                if (File.Exists(excelFile))
                {
                    MessageBox.Show("Baza de date a fost exportata pe perioada selectata in fisierul 'Export.xls' pe care il gasesti pe Desktop.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // KryptonMessageBox.Show("Baza de date a fost exportata pe perioada selectata in fisierul 'Export.xls' pe care il gasesti pe Desktop.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}