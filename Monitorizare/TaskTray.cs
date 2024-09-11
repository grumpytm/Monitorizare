using System.ComponentModel;
using Cron;

namespace Monitorizare
{
    public class TaskTray : ApplicationContext
    {
        private NotifyIcon notifyIcon = new NotifyIcon();

        /* crontab daemon */
        private static readonly CronDaemon cron_daemon = new CronDaemon();
        private static CronTasks CronTask = new CronTasks();

        public TaskTray()
        {
            ContextMenuStrip ContextMenu = new ContextMenuStrip();
            ContextMenu.Items.Add(new ToolStripMenuItem("Vizualizare", null, ShowViewData));
            ContextMenu.Items.Add(new ToolStripMenuItem("Exportare", null, ShowExportData));
            ContextMenu.Items.Add("-");
            ContextMenu.Items.Add(new ToolStripMenuItem("Mesaje", null, ShowLogs));
            ContextMenu.Items.Add(new ToolStripMenuItem("Setari", null, ShowSettings));

            // disabled
            // ContextMenu.Items.Add("-");
            // ContextMenu.Items.Add(new ToolStripMenuItem("Descarcare", null, DownloadNow));

            // work in progress
            // ContextMenu.Items.Add("-");
            // ContextMenu.Items.Add(new ToolStripMenuItem("Format logs", null, FormatLogs));

            ContextMenu.Items.Add("-");
            ContextMenu.Items.Add(new ToolStripMenuItem("Inchide", null, Exit));

            notifyIcon.ContextMenuStrip = ContextMenu;

            // notifyIcon.ContextMenu.MenuItems[3].Enabled = false; // Logs
            notifyIcon.ContextMenuStrip.Items[4].Enabled = false; // Setari

            /*
            ContextMenuStrip myMenu = new ContextMenuStrip();
            var item = new ToolStripMenuItem("test");
            item.Image = Properties.Resources.book;
            item.Click += new EventHandler(ShowViewData);
            myMenu.Items.Add(item);
            myMenu.Items.Add("-");
            notifyIcon.ContextMenuStrip = myMenu;
            */

            notifyIcon.Visible = true;
            notifyIcon.Text = "Monitorizare incarcare/descarcare moara.";
            notifyIcon.Icon = Resources.database;
            notifyIcon.DoubleClick += new EventHandler(ShowViewData);
            // notifyIcon.Click += new EventHandler(ShowViewData);

            /* Setup and start crontab daemon */
            // cron_daemon.Add("0 */1 * * *", ContabTask);
            // cron_daemon.Start();
        }

        private static async void FormatLogs(object sender, EventArgs e)
        {
            string[] logFiles = {
                "Incarcare.log",
                "Descarcare.log"
            };

            await CronTasks.ProcessLogFilesAsync(logFiles);
        }

        private void Exit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        public void ContabTask()
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s, e) =>
            {
                CronTask.ContabTask();
            };

            bg.RunWorkerAsync();
            while (bg.IsBusy)
            {
                Application.DoEvents(); //processes all windows messages currently in the message queue
            }
        }

        private void ShowViewData(object sender, EventArgs e)
        {
            bool isFormOpen = Functions.IsAlreadyOpen(typeof(Vizualizare));
            if (isFormOpen == false)
            {
                Vizualizare viewWindow = new Vizualizare();
                viewWindow.Show();
                viewWindow.BringToFront();
                viewWindow.Activate();
            }
        }

        private void ShowExportData(object sender, EventArgs e)
        {
            bool isFormOpen = Functions.IsAlreadyOpen(typeof(Exporta));
            if (isFormOpen == false)
            {
                Exporta exportForm = new Exporta();
                exportForm.Show();
                exportForm.BringToFront();
                exportForm.Activate();
            }
        }

        private void DownloadNow(object sender, EventArgs e)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s, x) =>
            {
                CronTask.ContabTask();
            };

            bg.RunWorkerAsync();
            while (bg.IsBusy)
            {
                Application.DoEvents(); //processes all windows messages currently in the message queue
            }
        }

        private void ShowSettings(object sender, EventArgs e)
        {

        }

        private void ShowLogs(object sender, EventArgs e)
        {
            bool isFormOpen = Functions.IsAlreadyOpen(typeof(Logs));
            if (isFormOpen == false)
            {
                Logs logsForm = new Logs();
                logsForm.Show();
                logsForm.BringToFront();
                logsForm.Activate();
            }
        }
    }
}