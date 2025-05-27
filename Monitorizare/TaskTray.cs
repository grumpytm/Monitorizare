using Cron;

namespace Monitorizare;

public class TaskTray : ApplicationContext
{
    private CronTasks _cronTasks;
    private readonly NotifyIcon _notifyIcon;
    private readonly CronDaemon _cronDaemon;
    private readonly IAppSettings _settings;

    public TaskTray()
    {
        _cronTasks = new CronTasks();
        _cronDaemon = new CronDaemon();
        _settings = new AppSettings();

        // Notification icon setup
        _notifyIcon = new NotifyIcon
        {
            ContextMenuStrip = CreateContextMenu(),
            Icon = Resources.Database,
            Visible = true,
        };

        _notifyIcon.DoubleClick += (s, e) => ShowForm<Vizualizare>();

        // Database migrations aka. integrity check manage future changes to the database schema
        _ = DatabaseFactory.GetDatabase().ApplyMigrationsAsync();

        // Setup and start Crontab daemon
        var schedule = GetSchedule();
        _cronDaemon.Add(schedule, async () => await _cronTasks.SaveTransportLogs());
        _cronDaemon.Start();
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.AddRange(MenuItems);
        return menu;
    }

    private ToolStripItem[] MenuItems => new ToolStripItem[]
    {
        new ToolStripMenuItem("Vizualizare", null, (s, e) => ShowForm<Vizualizare>()),
        new ToolStripMenuItem("Exportare", null, (s, e) => ShowForm<Exporta>()),
        new ToolStripSeparator(),
        new ToolStripMenuItem("Mesaje", null, (s, e) => ShowForm<Logs>()),
        new ToolStripSeparator(),
# if DEBUG
        new ToolStripMenuItem("Update logs", null, UpdateLogsAsync),
        new ToolStripSeparator(),
# endif
        new ToolStripMenuItem("Inchide", Resources.Door.ToBitmap(), (s, e) => ExitThread())
    };

    private string GetSchedule() =>
        _settings.Configuration.GetSection("General:Schedule").Value ?? "0 */1 * * *"; // Run once every hour by default

    protected override void Dispose(bool disposing)
    {
        if (disposing) _notifyIcon.Dispose();
        base.Dispose(disposing);
    }

    // Generic function to create a new form or bring it to front
    private static void ShowForm<T>() where T : Form, new()
    {
        var existingForm = Application.OpenForms[typeof(T).Name];
        if (existingForm is null)
        {
            var form = new T
            {
                Icon = Resources.Database
            };
            form.Show();
            form.Activate();
        }
        else
        {
            existingForm.Activate();
            existingForm.BringToFront();
        }
    }

# if DEBUG // Debugging purposes only
    public async void UpdateLogsAsync(object? sender, EventArgs e) =>
        await _cronTasks.SaveTransportLogs();
# endif
}