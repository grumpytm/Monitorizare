using System;
using System.Net.Sockets;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Linq;

/* 3rd party libs */
using LumenWorks.Framework.IO.Csv;
using System.Data.SQLite;
// using FTPClient;

namespace Monitorizare
{
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

        /* Data tables */
        public DataTable Incarcare { get; set; }
        public DataTable Descarcare { get; set; }

        /* Import filters */
        public class DataFilters
        {
            public int Incarcare { get; set; }
            public int Descarcare { get; set; }
        }

        public void ContabTask()
        {
            /* Ping test */
            bool hasPing = Functions.PingHost(Host, Timeout);
            if (hasPing)
            {
                try
                {
                    /* Remove older files first */
                    RemoveOlder();

                    /* Fetch files from FTP server */
                    FetchFiles(Files);

                    RemoveTrailing(Files);

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
                    Incarcare = ParseToDatatable("Incarcare.log", h1);

                    string[] h2 = { "ziua", "data", "siloz", "greutate", "hala", "buncar" };
                    Descarcare = ParseToDatatable("Descarcare.log", h2);

                    /* Check database structure and integrity */
                    MySQLite.CheckIntegrity();

                    /* Import data table */
                    MySQLite.ImportToDatabase("incarcare", Incarcare);
                    MySQLite.ImportToDatabase("descarcare", Descarcare);

                    /* Unset the Datatables */
                    Incarcare.Clear();
                    Descarcare.Clear();

                    /* remove */
                    RemoveOlder();

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

        /* Remove trailing white space */
        public void RemoveTrailing(string[] files)
        {
            char[] trim = new char[] { '\t', ',', ' ' };
            foreach (var file in files)
            {
                List<string> lines = new List<string>();
                if (File.Exists(@file))
                {

                    using (StreamReader reader = File.OpenText(file))
                    {
                        while (!reader.EndOfStream)
                            lines.Add(reader.ReadLine().Replace(";", ",").TrimEnd(trim));
                    }
                    System.IO.File.WriteAllLines(file, lines.ToArray());
                }
                else
                {
                    // report missing file?
                    Console.WriteLine(String.Format("Missing {0} file..", file));
                    Logs.NewRecord(String.Format("Missing {0} file..", file));
                }
            }
        }

        public DataTable ParseToDatatable(string file, string[] headers)
        {
            int error = 0;

            /* Replace headers in file */
            var allLines = File.ReadAllLines(file);
            allLines[0] = String.Join(",", headers);
            File.WriteAllLines(file, allLines);

            /* Datatable headers */
            DataTable dt = new DataTable();
            
            foreach (var header in headers)
                dt.Columns.Add(header);

            /* Parse CSV log file */
            Console.WriteLine(String.Format("Parsing {0} file..", file));
            using (CsvReader csv = new CsvReader(new StreamReader(file), true))
            {
                csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;
                csv.DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine;
                string[] h = csv.GetFieldHeaders();

                int hLen = h.Length;

                if (csv.FieldCount != hLen)
                {
                    error++;
                    Console.WriteLine(String.Format("Error while trying to import from {0} file with only {1} records..", file, csv.FieldCount));
                    Logs.NewRecord(String.Format("Error while trying to import from {0} file with only {1} records..", file, csv.FieldCount));
                }
                else
                {
                    try
                    {
                        string[] currentRow = new string[csv.FieldCount];
                        while (csv.ReadNextRecord())
                        {
                            csv.CopyCurrentRecordTo(currentRow);
                            var line = string.Join(csv.Delimiter.ToString(), currentRow);

                            int count = line.Split(',').Length;
                            DataRow row = dt.NewRow();
                            if (count == hLen)
                            {
                                row[0] = Functions.Date2Timestamp(string.Concat(csv[0], " ", csv[1]), "dd/MM/yyyy H:mm");
                                for (int x = 1; x < (hLen - 1); x++)
                                    row[x] = csv[x + 1];
                                dt.Rows.Add(row);
                            }
                            else
                            {
                                Console.WriteLine(String.Format("Row in wrong format? Count: {0} | Data: {1}", count, line));
                                Logs.NewRecord(String.Format("Row in wrong format? Count: {0} | Data: {1}", count, line));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: {0}\r\n | Stack trace: {1}", e.Message, e.StackTrace);
                        Logs.NewRecord(String.Format("Exception: {0}\r\n | Stack trace: {1}", e.Message, e.StackTrace));
                    }
                    finally
                    {
                        Console.WriteLine(String.Format("Datatable #1 row count: {0}", dt.Rows.Count));
                        Logs.NewRecord(String.Format("Datatable #1 row count: {0}", dt.Rows.Count));
                    }
                }
            }
            return dt;
        }
    }
}