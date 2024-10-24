using Cron;
using System.Windows.Forms;

namespace Monitorizare
{
    public class TaskTray : IDisposable
    {
        private CronTasks _cronTasks;
        private NotifyIcon _notifyIcon;

        private static readonly CronDaemon _cronDaemon = new(); /* crontab daemon */

        public TaskTray()
        {
            _cronTasks = new CronTasks();

            _notifyIcon = new NotifyIcon
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Resources.database,
                Visible = true,
            };

            _notifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Vizualizare", null, ShowViewData),
                new ToolStripMenuItem("Exportare", null, ShowExportData),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Mesaje", Resources.sliders.ToBitmap(), ShowLogs),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Update logs", Resources.clock.ToBitmap(), UpdateLogsAsync),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Inchide", Resources.door.ToBitmap(), Exit)
            });

            /* Setup and start crontab daemon */
            // _cronDaemon.Add("0 */1 * * *", ContabTask);
            // _cronDaemon.Start();

            _notifyIcon.DoubleClick += new EventHandler(ShowViewData);
            Application.ApplicationExit += OnApplicationExit;
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

        private void ShowViewData(object? sender, EventArgs e)
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

        private void ShowExportData(object? sender, EventArgs e)
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

        private void ShowLogs(object? sender, EventArgs e)
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

        public async void UpdateLogsAsync(object? sender, EventArgs e)
        {
            await _cronTasks.SaveTransportLogs();
        }

        private void Exit(object? sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            Application.Exit();
        }

        private void OnApplicationExit(object? sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }
    }
}