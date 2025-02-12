using Cron;
using Monitorizare.Records;
using Monitorizare.Settings;

namespace Monitorizare;

public class TaskTray : IDisposable
{
    private CronTasks _cronTasks;
    private NotifyIcon _notifyIcon;
    private readonly CronDaemon _cronDaemon;
    private readonly IAppSettings _settings;

    public TaskTray()
    {
        _cronTasks = new CronTasks();
        _cronDaemon = new CronDaemon();
        _settings = new AppSettings();

        _notifyIcon = new NotifyIcon
        {
            ContextMenuStrip = new ContextMenuStrip(),
            Icon = Resources.database,
            Visible = true,
        };

        // Context menu
        _notifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
        {
             new ToolStripMenuItem("Vizualizare", null, ShowViewData),
            new ToolStripMenuItem("Exportare", null, ShowExportData),
            new ToolStripSeparator(),
            new ToolStripMenuItem("Mesaje", null, ShowLogs),
            new ToolStripSeparator(),
            new ToolStripMenuItem("Update logs", null, UpdateLogsAsync),
            new ToolStripSeparator(),
            new ToolStripMenuItem("Inchide", Resources.door.ToBitmap(), Exit)
        });

        _notifyIcon.DoubleClick += new EventHandler(ShowViewData);

        // Setup and start Crontab daemon
        _cronDaemon.Add(GetSchedule(), async () => await _cronTasks.SaveTransportLogs());
        _cronDaemon.Start();

        Application.ApplicationExit += OnApplicationExit;
    }

    private string GetSchedule() =>
        _settings.Configuration.GetSection("General:Schedule").Value ?? "0 */1 * * *";

    private void ShowViewData(object? sender, EventArgs e)
    {
        ShowForm<Vizualizare>();
    }

    private void ShowExportData(object? sender, EventArgs e)
    {
        ShowForm<Exporta>();
    }

    private void ShowLogs(object? sender, EventArgs e)
    {
        ShowForm<Logs>();
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

    // Generic function to create a new form or bring it to front
    private void ShowForm<T>() where T : Form, new()
    {
        var existingForm = Application.OpenForms[typeof(T).Name];
        if (existingForm == null)
        {
            var form = new T();
            form.Show();
            form.Activate();
        }
        else
        {
            existingForm.BringToFront();
        }
    }
}