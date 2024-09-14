using System.Data;
using System.Diagnostics;

using Monitorizare.Services;

namespace Monitorizare;

class CronTasks
{
    /* Settings */
    
    /*
        String host = "10.0.0.11";
        String user = "anonymous";
        String pass = "anonymous";
    */

    String host = "192.168.1.11";
    String user = "USER";
    String pass = "USER";

    String path = "usr/Log/";

    int port = 21;
    int timeout = 3000;

    /* Files */
    string[] files = {
        "Incarcare.log",
        "Descarcare.log"
    };

    public string Host { get => host; set => host = value; }
    public int Port { get => port; set => port = value; }
    public int Timeout { get => timeout; set => timeout = value; }

    private string User { get => user; set => user = value; }
    private string Pass { get => pass; set => pass = value; }
    private string Path { get => path; set => path = value; }
    public string[] Files { get => files; set => files = value; }

    /* work in progress */
    public static async Task ProcessLogFilesAsync()
    {
        var logFiles = SettingsProvider.GetLogFiles();

        var result = await TransportService.SaveLogsToDatabaseAsync(logFiles);

        var (affectedRows, recordsCount) = result;

        string pluralRows = GetPluralFromCount(affectedRows);
        string pluralCount = GetPluralFromCount(recordsCount);

        Debug.WriteLine($"Transport service: {recordsCount} record{pluralRows} parsed, {affectedRows} row{pluralRows} saved to database..");
    }

    private static string GetPluralFromCount(int count)
    {
        var plural = count == 1 ? "" : "s";
        return plural;
    }

    /* old stuff */
    public void ContabTask()
    {
        /* Ping test */
        //bool hasPing = Functions.PingHost(Host, Timeout);
        bool hasPing = true; // to be removed!

        if (hasPing)
        {
            try
            {
                /* Remove older files first */
                //RemoveOlder();

                /* Fetch files from FTP server */
                //FetchFiles(Files);

                //RemoveTrailing(Files);

                int missing = 0;
                foreach (var file in files)
                {
                    if (!File.Exists(@file))
                        missing++;
                }

                if (missing > 0)
                    return;

                 /* Parse CSV into a Datatable */
                 string[] h1 = { "ziua", "data", "siloz", "greutate" };
                // Incarcare = ParseToDatatable("Incarcare.log", h1);

                string[] h2 = { "ziua", "data", "siloz", "greutate", "hala", "buncar" };
                // Descarcare = ParseToDatatable("Descarcare.log", h2);

                //FormatLogFiles(files);

                /*
                // Check database structure and integrity
                // MySQLite.CheckIntegrity(); // removed

                // Import data table
                //xx MySQLite.ImportToDatabase("incarcare", Incarcare);
                //xx MySQLite.ImportToDatabase("descarcare", Descarcare);

                // Unset the Datatables
                Incarcare.Clear();
                Descarcare.Clear();

                RemoveOlder(); // remove
                */

            }
            catch (System.FormatException ex)
            {
                Console.WriteLine(String.Format("Exception: {0}", ex.Message));
                Logs.NewRecord(String.Format("Exception: {0}", ex.Message));
            }
        }
        else
        {
            Console.WriteLine(String.Format("Ping on {0} failed..", Host));
            Logs.NewRecord(String.Format("Ping on {0} failed..", Host));
        }
    }

    private void RemoveOlder()
    {
        foreach (var file in files)
        {
            if (File.Exists(@file))
                File.Delete(@file);
        }
    }

    public int FetchFiles(string[] files)
    {
        int error = 0;
        FtpClient ftp = new FtpClient(Host, User, Pass);
        try
        {
            ftp.Login();
            ftp.ChangeDir(Path);
            foreach (var file in files)
                ftp.Download(file);
        }
        catch (FtpClient.FtpException ex)
        {
            error++;
            Console.WriteLine("Error: {0}", ex.Message);
            Logs.NewRecord(String.Format("Error: {0}", ex.Message));
        }
        finally
        {
            ftp.Close();
        }
        return error;
    }

    /* to be removed */
    public DataTable ParseToDatatable(string file, string[] headers)
    {
        DataTable dt = new DataTable();
        return dt;
    }
}