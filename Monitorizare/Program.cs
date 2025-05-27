namespace Monitorizare;

class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Mutex lock
        var mutexName = "7d4c3b15-cda8-42c6-add8-954acd4e136c"; // Random GUID generated with Guid.NewGuid().ToString();

        using var mutex = new Mutex(true, mutexName, out bool createdNew);
        if (!createdNew)
        {
            MessageBox.Show("Aplicatia este deja pornita!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Start application normally
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Start the task tray application
        using var taskTray = new TaskTray();
        Application.Run(taskTray);
    }
}