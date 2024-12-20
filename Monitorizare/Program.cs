using Monitorizare.Database;

namespace Monitorizare;

class Program
{
    // private static TaskTray _taskTray = new();

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static async Task Main()
    {
        /* Mutex lock */
        var mutexName = "7d4c3b15-cda8-42c6-add8-954acd4e136c"; // Random guid via Guid.NewGuid().ToString();

        using (var mutex = new Mutex(true, mutexName, out bool createdNew))
        {
            if (!createdNew)
            {
                MessageBox.Show("Aplicatia este deja pornita!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Perform a database integrity check
            await new DatabaseUtils().DatabaseIntegrityCheck();

            // Start application normally
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start the task tray application
            using (TaskTray taskTray = new TaskTray())
            {
                Application.Run();
            }
        }
    }
}