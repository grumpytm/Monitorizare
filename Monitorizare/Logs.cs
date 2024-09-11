using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* 3rd party libs */

using MetroFramework.Forms;

namespace Monitorizare
{
    public partial class Logs : MetroForm
    {
        public Logs()
        {
            InitializeComponent();
            MySQLite.CheckIntegrity();

            this.Icon = Resources.database;
            kryptonDataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 234, 234, 234);

            // SanityCheck();
            InitializeDataGridView();
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

        public static void NewRecord(string message)
        {
            long date = Functions.UnixTimeNow();
            //xx MySQLite.NewLogRecod(date, message);
            // Console.WriteLine(String.Format("Now: {0} | message: {1}", date, message));
        }

        public void InitializeDataGridView()
        {
            kryptonDataGridView1.DataSource = MySQLite.GetLogs();
            kryptonDataGridView1.DataBindingComplete += GridView_DataBindingComplete;
            kryptonDataGridView1.Columns["Data"].DefaultCellStyle.Format = "dd.MM.yyyy";

            kryptonDataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // kryptonDataGridView1.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            kryptonDataGridView1.Columns["#"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            kryptonDataGridView1.Columns["Data"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            kryptonDataGridView1.Columns["Ora"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            kryptonDataGridView1.Columns["Log"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            List<int> widths = new List<int>(new int[] { 50, 80, 60, 400 });

            int count = kryptonDataGridView1.ColumnCount;

            for (int x = 0; x < count; x++)
                kryptonDataGridView1.Columns[x].Width = widths[x];
        }

        private void GridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ((DataGridView)sender).ClearSelection();
        }
    }
}