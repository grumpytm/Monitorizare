using Cron;
using System.ComponentModel;

namespace Monitorizare
{
    public class TaskTray : ApplicationContext
    {
        private CronTasks _cronTasks;
        private NotifyIcon notifyIcon = new NotifyIcon();
        private static readonly CronDaemon cron_daemon = new CronDaemon(); /* crontab daemon */

        public TaskTray()
        {
            _cronTasks = new CronTasks();
            BuildContextMenu();
        }

        public void BuildContextMenu()
        {
            ContextMenuStrip ContextMenu = new ContextMenuStrip();
            ContextMenu.Items.Add(new ToolStripMenuItem("Vizualizare", null, ShowViewData));
            ContextMenu.Items.Add(new ToolStripMenuItem("Exportare", null, ShowExportData));
            ContextMenu.Items.Add("-");
            ContextMenu.Items.Add(new ToolStripMenuItem("Mesaje", null, ShowLogs));
            // ContextMenu.Items.Add(new ToolStripMenuItem("Setari", null, ShowSettings));

            // work in progress
            ContextMenu.Items.Add("-");

            ContextMenu.Items.Add(new ToolStripMenuItem("Update logs", null, UpdateLogsAsync));

            // disabled
            // ContextMenu.Items.Add("-");
            // ContextMenu.Items.Add(new ToolStripMenuItem("Descarcare", null, DownloadNow));

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
            notifyIcon.Text = "Monitorizare moara";
            notifyIcon.Icon = Resources.database;
            notifyIcon.DoubleClick += new EventHandler(ShowViewData);
            // notifyIcon.Click += new EventHandler(ShowViewData);

            /* Setup and start crontab daemon */
            // cron_daemon.Add("0 */1 * * *", ContabTask);
            // cron_daemon.Start();
        }

        public async void UpdateLogsAsync(object sender, EventArgs e)
        {
            await _cronTasks.TrySaveTransportLogs();
        }

        private void Exit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        /*
        public void ContabTask()
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s, e) =>
            {
                _cronTasks.ContabTask();
            };

            bg.RunWorkerAsync();
            while (bg.IsBusy)
            {
                Application.DoEvents(); //processes all windows messages currently in the message queue
            }
        }
        */

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
            // not implemented yet
        }

        private void ShowSettings(object sender, EventArgs e)
        {
            // not implemented yet
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