using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace Monitorizare
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // AppDomain.CurrentDomain.AppendPrivatePath("lib");

            using (SingleInstanceMutex sim = new SingleInstanceMutex())
            {
                if (sim.IsOtherInstanceRunning)
                {
                    MessageBox.Show("Aplicatia este deja pornita!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Initialize program here.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Instead of running a form, we run an ApplicationContext.
                Application.Run(new TaskTray());
            }
        }
    }
}