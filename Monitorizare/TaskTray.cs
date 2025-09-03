using Cron;

namespace Monitorizare;

public class TaskTray : ApplicationContext
{
    private readonly CronTasks _cronTasks;
    private readonly NotifyIcon _notifyIcon;
    private readonly CronDaemon _cronDaemon;
    private readonly IAppSettings _settings;
    private readonly string DefaultCron = "0 */1 * * *"; // By default run once every hour

    public TaskTray()
    {
        _cronTasks = new CronTasks();
        _cronDaemon = new CronDaemon();
        _settings = new AppSettings();

        // Notification icon setup
        _notifyIcon = new()
        {
            ContextMenuStrip = CreateContextMenu(),
            Icon = Resources.Database,
            Visible = true,
        };

        _notifyIcon.DoubleClick += (s, e) => ShowForm<Vizualizare>();

        // Database migrations aka. integrity check and manage future changes done to the database schema
        _ = DatabaseFactory.GetDatabase().ApplyMigrationsAsync();

        // Setup and start Crontab daemon
        var schedule = _settings.Configuration.GetSection("General:Schedule").Value ?? DefaultCron;
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
        new ToolStripMenuItem("Administrare")
        {
            DropDownItems =
            {
                new ToolStripMenuItem("Mesaje", null, (s, e) => ShowForm<Logs>()),
                // new ToolStripMenuItem("Migratii", null, (s, e) => MessageBox.Show("Show migrations")), // To do!
#if DEBUG // Debugging purposes only
                new ToolStripSeparator(),
                new ToolStripMenuItem("Update logs", null, UpdateLogsAsync), // Update logs on-demand
# endif
            }
        },

        new ToolStripSeparator(),
        new ToolStripMenuItem("Inchide", Resources.Door.ToBitmap(), (s, e) => ExitThread())
    };

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
            var form = new T { Icon = Resources.Database };
            form.Show();
            form.Activate();
        }
        else
        {
            existingForm.Activate();
            existingForm.BringToFront();
        }
    }

#if DEBUG // Debugging purposes only
    public async void UpdateLogsAsync(object? sender, EventArgs e) =>
        await _cronTasks.SaveTransportLogs();
#endif
}