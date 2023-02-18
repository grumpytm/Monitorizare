using System;
using System.ComponentModel;
using System.Windows.Forms;

/* 3rd party libs */
using CronNET;

namespace Monitorizare
{
    public class TaskTray : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();

        /* crontab daemon */
        private static readonly CronDaemon cron_daemon = new CronDaemon();
        private static CronTasks CronTask = new CronTasks();

        public TaskTray()
        {
            ContextMenu ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem("Vizualizare", ShowViewData));
            ContextMenu.MenuItems.Add(new MenuItem("Exportare", ShowExportData));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("Mesaje", ShowLogs));
            ContextMenu.MenuItems.Add(new MenuItem("Setari", ShowSettings));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("Descarcare", DownloadNow));
            ContextMenu.MenuItems.Add("-");
            ContextMenu.MenuItems.Add(new MenuItem("Inchide", Exit));

            notifyIcon.ContextMenu = ContextMenu;

            // notifyIcon.ContextMenu.MenuItems[3].Enabled = false; // Logs
            notifyIcon.ContextMenu.MenuItems[4].Enabled = false; // Setari

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
            notifyIcon.Icon = Monitorizare.Properties.Resources.database;
            notifyIcon.DoubleClick += new EventHandler(ShowViewData);
            // notifyIcon.Click += new EventHandler(ShowViewData);

            /* Setup and start crontab daemon */
            cron_daemon.AddJob("0 */1 * * *", ContabTask);
            //cron_daemon.AddJob("* * * * *", ContabTask);
            cron_daemon.Start();
        }

        void Exit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        public void ContabTask()
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s, e) => {
                CronTask.ContabTask();
            };

            bg.RunWorkerAsync();
            while (bg.IsBusy)
            {
                Application.DoEvents(); //processes all windows messages currently in the message queue
            }
        }

        void ShowViewData(object sender, EventArgs e)
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

        void ShowExportData(object sender, EventArgs e)
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

        void DownloadNow(object sender, EventArgs e)
        {

            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s, x) => {
                CronTask.ContabTask();
            };

            bg.RunWorkerAsync();
            while (bg.IsBusy)
            {
                Application.DoEvents(); //processes all windows messages currently in the message queue
            }
        }

        void ShowSettings(object sender, EventArgs e)
        {
            
        }

        void ShowLogs(object sender, EventArgs e)
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
 