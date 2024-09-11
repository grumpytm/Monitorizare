using Monitorizare.Database;

namespace Monitorizare
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            var mutexName = "7d4c3b15-cda8-42c6-add8-954acd4e136c"; // Random guid via Guid.NewGuid().ToString();

            using (var mutex = new Mutex(true, mutexName, out bool createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("Aplicatia este deja pornita!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                /* Database integrity check */
                await DatabaseService.CheckDatabaseIntegrity();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TaskTray());
            }
        }
    }
}