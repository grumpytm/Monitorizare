using System;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;

namespace Monitorizare
{
    class Functions
    {
        static Timer myTimer = new Timer();
        static readonly Random rand = new Random();

        // https://stackoverflow.com/a/13740781
        private static void TimedStuff()
        {
            myTimer.Tick += (o, ea) =>
            {
                // do something
            };
            myTimer.Interval = 5000; // 5 seconds
            myTimer.Start();
        }

        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        /* Date to Timestamp */
        public static double Date2Timestamp(string date_string, string date_format)
        {
            DateTime date = DateTime.ParseExact(date_string, date_format, null);
            Double timestamp = Math.Truncate((date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            return timestamp;
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static bool PingHost(String host, int timeout)
        {
            try
            {
                // int timeout = 1000;
                Ping myPing = new Ping();
                byte[] buffer = new byte[32];
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                    return true;
                else
                    return false;
            }
            catch (System.Net.Sockets.SocketException)
            {
                // Console.WriteLine("Failed to PING host...");
                return false;
            }
        }

        public static bool IsPortOpen(string host, int port)
        {
            try
            {
                using (TcpClient client = new TcpClient(host, port)) {
                    return true;
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(String.Format("Exception: {0}", ex));
                return false;
            }
        }

        public static bool IsAlreadyOpen(Type formType)
        {
            bool isOpen = false;
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == formType)
                {
                    isOpen = true;
                    form.BringToFront();
                    form.WindowState = FormWindowState.Normal;
                    form.Activate();
                }
            }
            return isOpen;
        }

        protected bool CheckDate(String value)
        {
            if (DateTime.TryParse(value, out DateTime DT) == true)
                return true;
            return (TimeSpan.TryParse(value, out TimeSpan TS) == true) ? true : false;
        }

        public static bool IsFileLocked(string file)
        {
            if (File.Exists(file))
            {
                FileStream stream = null;
                try
                {
                    FileInfo f = new FileInfo(file);
                    stream = f.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (IOException)
                {
                    return true;
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }
            return false;
        }

        public static void DoTask()
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (s, e) => {
                // action here
            };

            bg.RunWorkerCompleted += (s, e) =>
            {
                //when download is completed
            };

            bg.RunWorkerAsync();
            while (bg.IsBusy)
            {
                Application.DoEvents(); //processes all windows messages currently in the message queue
            }
        }
    }
}