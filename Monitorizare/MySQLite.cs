using System.Data;
using System.Globalization;

/* 3rd party libs */
using System.Data.SQLite;

namespace Monitorizare
{
    class MySQLite
    {
        /* Database file */
        static string db = "records.db";

        public static string Database { get => db; set => db = value; }

        public static void CheckIntegrity()
        {
            if (!File.Exists(Database) || new FileInfo(Database).Length == 0)
            {
                using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Compress=True;", Database)))
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    try
                    {
                        using (SQLiteCommand command = new SQLiteCommand(connection))
                        {
                            /* incarcare */
                            command.CommandText = "CREATE TABLE IF NOT EXISTS `incarcare` (`id` INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, `data` INTEGER NOT NULL UNIQUE, `siloz` INTEGER NOT NULL, `greutate` INTEGER NOT NULL)";
                            command.ExecuteNonQuery();

                            /* descarcare */
                            command.CommandText = "CREATE TABLE IF NOT EXISTS `descarcare`(`id` INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, `data` INTEGER NOT NULL UNIQUE, `siloz` INTEGER NOT NULL, `greutate` INTEGER NOT NULL, `hala` INTEGER NOT NULL, `buncar` INTEGER NOT NULL)";
                            command.ExecuteNonQuery();

                            /* logs */
                            command.CommandText = "CREATE TABLE IF NOT EXISTS `logs` (`id` INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, `data` INTEGER NOT NULL, `msg` TEXT NOT NULL)";
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Exception: {0}\r\n | Stack trace: {1}", e.Message, e.StackTrace);
                        if (transaction != null)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (SQLiteException)
                            {
                                Console.WriteLine("Transaction rollback failed.");
                            }
                        }
                    }
                    finally
                    {
                        transaction.Commit();
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }

                    connection.Dispose();
                }
            }
        }

        public static List<DateTime> SetMinMaxData(string table)
        {
            List<DateTime> range = new List<DateTime>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Compress=True;", Database)))
                {
                    connection.Open();

                    /* min & max incarcare */
                    string query = String.Format("SELECT strftime('%d.%m.%Y', datetime(MIN(data), 'unixepoch', 'localtime')) AS min, strftime('%d.%m.%Y', datetime(MAX(data), 'unixepoch', 'localtime')) AS max FROM {0}", table);
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader.HasRows && reader.GetValue(0) != DBNull.Value)
                            {
                                DateTime min = DateTime.ParseExact(reader["min"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
                                DateTime max = DateTime.ParseExact(reader["max"].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture);

                                range.Add(min);
                                range.Add(max);
                            }
                        }
                    }
                    if (connection.State == ConnectionState.Open)
                        connection.Close();

                    connection.Dispose();
                }
            }
            catch (FormatException err)
            {
                Console.WriteLine(String.Format("Exception: {0}\r\n | Stack trace: {1}", err.Message, err.StackTrace));
                Logs.NewRecord(String.Format(String.Format("Exception: {0}\r\n | Stack trace: {1}", err.Message, err.StackTrace)));
            }
            return range;
        }

        public static List<DateTime> SetMinMaxDataMultiple()
        {
            List<DateTime> range = new List<DateTime>();

            var dict = new Dictionary<int, string> {
                { 1, "a" }, // min incarcare
                { 2, "b" }, // max incarcare
                { 3, "c" }, // min descarcare
                { 4, "d" }, // max descarcare
            };

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Compress=True;", Database)))
                {
                    connection.Open();

                    /* Build query for min & max */
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
                    string query = "SELECT" +
                        "(SELECT MIN(data) FROM incarcare) AS a," +
                        "(SELECT MAX(data) FROM incarcare) AS b," +
                        "(SELECT MIN(data) FROM descarcare) AS c," +
                        "(SELECT MAX(data) FROM descarcare) AS d";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            foreach (KeyValuePair<int, string> entry in dict)
                            {
                                double.TryParse(reader[entry.Value].ToString(), out double timestamp);
                                range.Add(epoch.AddSeconds(timestamp));
                            }
                        }
                    }
                    if (connection.State == ConnectionState.Open)
                        connection.Close();

                    connection.Dispose();
                }
            }
            catch (FormatException err)
            {
                Console.WriteLine(String.Format("Exception: {0}\r\n | Stack trace: {1}", err.Message, err.StackTrace));
                Logs.NewRecord(String.Format("Exception: {0}\r\n | Stack trace: {1}", err.Message, err.StackTrace));
            }
            return range;
        }

        public static DataTable FetchTable(string table, Dictionary<int, string> columnsDict, int count)
        {
            List<DateTime> values = SetMinMaxData(table);

            string dateMin = values[0].ToString("dd.MM.yyyy");
            string dateMax = values[1].ToString("dd.MM.yyyy");

            // Console.WriteLine(String.Format("Table: {0} - Min: {1} | Max: {2}", table, dateMin, dateMax));

            DataTable dt = FillTable(dateMin, dateMax, table, columnsDict, count);
            return dt;
        }

        public static DataTable FetchTable(string dateMin, string dateMax, string table, Dictionary<int, string> columnsDict, int count)
        {
            DataTable dt = FillTable(dateMin, dateMax, table, columnsDict, count);
            return dt;
        }


        public static DataTable FillTable(string dateMin, string dateMax, string table, Dictionary<int, string> columnsDict, int count)
        {
            /* Data tables */
            DataTable dt = new DataTable();
            DataTable filtered = new DataTable();

            string[] columns = columnsDict.Values.Take(count).ToArray();

            /* Column names */
            dt.Columns.Add("Timestamp", typeof(string));
            dt.Columns.Add("Data", typeof(DateTime));
            dt.Columns.Add("Ora", typeof(string));

            foreach (string element in columns.Skip(3))
                dt.Columns.Add(element, typeof(Int16));

            /* Swap Timestamp with # in GridView columns */
            columnsDict[1] = "#";
            string[] selectedColumns = columnsDict.Values.Take(dt.Columns.Count).ToArray();

            /* Empty query */
            string query = string.Empty;

            var tuple = Tuple.Create(
                Functions.Date2Timestamp(dateMin, "dd.MM.yyyy"),
                Functions.Date2Timestamp(dateMax, "dd.MM.yyyy")
            );

            double min = Math.Min(tuple.Item1, tuple.Item2);
            double max = Math.Max(tuple.Item1, tuple.Item2);

            /* Selected columns */
            string colSel = String.Join(",", columns.Skip(3)).ToLower();

            /* Build SQL query */
            if (min != max)
                query = string.Format("SELECT data, strftime('%d.%m.%Y', datetime(data, 'unixepoch', 'localtime')) AS ziua, strftime('%H:%M', datetime(data, 'unixepoch', 'localtime')) AS ora, {1} FROM {0} WHERE strftime('%Y-%m-%d', datetime(data, 'unixepoch', 'localtime')) BETWEEN strftime('%Y-%m-%d', datetime({2}, 'unixepoch', 'localtime')) AND strftime('%Y-%m-%d', datetime({3}, 'unixepoch', 'localtime')) ORDER BY ziua, ora DESC", table, colSel, min, max);
            else
                query = string.Format("SELECT data, strftime('%d.%m.%Y', datetime(data, 'unixepoch', 'localtime')) AS ziua, strftime('%H:%M', datetime(data, 'unixepoch', 'localtime')) AS ora, {1} FROM {0} WHERE ziua = strftime('%d.%m.%Y', datetime({2}, 'unixepoch', 'localtime')) ORDER BY ziua, ora DESC", table, colSel, min);

            /* Load data from SQLite */
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Compress=True;", Database)))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                {
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        object[] row = new object[reader.FieldCount];
                        reader.GetValues(row);

                        /* fix DateTime */
                        row[1] = DateTime.ParseExact(row[1].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture);

                        dt.Rows.Add(row);
                    }

                    reader.Close();
                }

                if (connection.State == ConnectionState.Open)
                    connection.Close();

                connection.Dispose();
            }

            if (dt.Rows.Count != 0)
            {
                /* Order the data in data table */
                // DataTable ordered = dt.AsEnumerable().OrderBy(r => r.Field<DateTime>("Data")).CopyToDataTable();

                /* Add index column with row number  */
                DataColumn index = new DataColumn("#", typeof(int))
                {
                    DefaultValue = "0"
                };

                dt.Columns.Add(index);

                for (int x = 0; x < dt.Rows.Count; x++)
                    dt.Rows[x]["#"] = x + 1;

                /* Filter out result */
                filtered = new DataView(dt).ToTable(false, selectedColumns);

                return filtered;
            }
            else
            {
                /* Add index column with row number  */
                DataColumn index = new DataColumn("#", typeof(int))
                {
                    DefaultValue = "0"
                };

                dt.Columns.Add(index);

                filtered = new DataView(dt).ToTable(false, selectedColumns);
                return filtered;
            }
        }

        /* Insert data from Datatabe into the SQLite table */
        public static void ImportToDatabase(string table, DataTable dt)
        {
            int imported = 0;
            int rowsAffected = 0;
            int total = dt.Rows.Count;
            if (dt.Rows.Count == 0) return;

            /* Stopwatch */
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Compress=True;", Database)))
            {
                connection.Open();
                SQLiteTransaction transaction = null;
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    try
                    {
                        transaction = connection.BeginTransaction();

                        /* set column names */
                        string columnNames = String.Join(",", dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray().Skip(1));

                        /* insert */
                        foreach (DataRow row in dt.Rows)
                        {
                            var paramValues = String.Join(",", row.ItemArray.AsEnumerable().Select(x => x.ToString()).ToList()).TrimEnd(',');

                            command.CommandText = string.Format("INSERT OR IGNORE INTO {0} ({1}) VALUES ({2})", table, columnNames, paramValues);
                            rowsAffected = command.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        imported = total;
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine(String.Format("Exception: {0}\r\n | Stack trace: {1}", e.Message, e.StackTrace));
                        Logs.NewRecord(String.Format("Exception: {0}\r\n | Stack trace: {1}", e.Message, e.StackTrace));
                        if (transaction != null)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (SQLiteException)
                            {
                                Console.WriteLine("Transaction rollback failed.");
                                Logs.NewRecord("Transaction rollback failed.");
                            }
                        }
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
                connection.Dispose();
            }
            sw.Stop();
            Console.WriteLine(String.Format("Imported {0} out of {1} rows successfully in {2} ms..", rowsAffected, imported, sw.ElapsedMilliseconds));
            Logs.NewRecord(String.Format("Imported {0} out of {1} rows successfully in {2} ms..", rowsAffected, imported, sw.ElapsedMilliseconds));
        }

        public static DataTable LoadLastRows(string table)
        {
            int max = 15;
            int count = (table == "incarcare") ? 5 : 7;

            /* Data tables */
            DataTable dt = new DataTable();
            DataTable filtered = new DataTable();

            /* DataTable columns */
            var columnsDict = new Dictionary<int, string> {
                { 1, "Timestamp" },
                { 2, "Data" },
                { 3, "Ora" },
                { 4, "Siloz" },
                { 5, "Greutate" },
                { 6, "Hala" },
                { 7, "Buncar" }
            };

            string[] columns = columnsDict.Values.Take(count).ToArray();

            /* Set column names */
            dt.Columns.Add("Timestamp", typeof(string));
            dt.Columns.Add("Data", typeof(DateTime));
            dt.Columns.Add("Ora", typeof(string));

            foreach (string element in columns.Skip(3))
                dt.Columns.Add(element, typeof(Int16));

            /* Swap Timestamp with # in GridView columns */
            columnsDict[1] = "#";
            string[] selectedColumns = columnsDict.Values.Take(dt.Columns.Count).ToArray();

            /* Set columns that will be fetched */
            string colSel = String.Join(",", columns.Skip(3)).ToLower();

            /* Build the query */
            string query = string.Format("SELECT data, strftime('%d.%m.%Y', datetime(data, 'unixepoch', 'localtime')) AS ziua, strftime('%H:%M', datetime(data, 'unixepoch', 'localtime')) AS ora, {1} FROM {0} ORDER BY id DESC LIMIT 0, {2}", table, colSel, max);

            /* Read data from SQLite */
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Compress=True;", Database)))
            {
                connection.Open();
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                    {
                        IDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            object[] row = new object[reader.FieldCount];
                            reader.GetValues(row);

                            /* Fix DateTime cell data */
                            row[1] = DateTime.ParseExact(row[1].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture);

                            dt.Rows.Add(row);
                        }
                        reader.Close();
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(String.Format("Exception: {0}\r\n | Stack trace: {1}", err.Message, err.StackTrace));
                    Logs.NewRecord(String.Format("Exception: {0}\r\n | Stack trace: {1}", err.Message, err.StackTrace));
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
                connection.Dispose();
            }

            if (dt.Rows.Count != 0)
            {
                /* Order the data in data table */
                // DataTable ordered = dt.AsEnumerable().OrderBy(r => r.Field<DateTime>("Data")).CopyToDataTable();

                /* Add index column with row number  */
                DataColumn index = new DataColumn("#", typeof(int))
                {
                    DefaultValue = "0"
                };

                dt.Columns.Add(index);

                for (int x = 0; x < dt.Rows.Count; x++)
                    dt.Rows[x]["#"] = x + 1;

                /* Filter out result */
                filtered = new DataView(dt).ToTable(false, selectedColumns);

                return filtered;
            }
            return dt;
        }

        /* Logs */

        public static void NewLogRecod(long date, string message)
        {
            CheckIntegrity();

            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Compress=True;", Database)))
            {
                connection.Open();
                SQLiteTransaction transaction = null;
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    try
                    {
                        transaction = connection.BeginTransaction();
                        command.CommandText = string.Format("INSERT into logs (data, msg) VALUES ({0}, '{1}')", date, message);
                        command.ExecuteNonQuery();
                    }
                    catch (SQLiteException e)
                    {
                        Console.WriteLine("Exception: {0}\r\n | Stack trace: {1}", e.Message, e.StackTrace);

                        if (transaction != null)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (SQLiteException)
                            {
                                Console.WriteLine("Transaction rollback failed.");
                            }
                        }
                    }
                    finally
                    {
                        transaction.Commit();
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
                connection.Dispose();
            }
        }

        public static DataTable GetLogs()
        {
            int max = 15;

            /* Build DataTable */
            DataTable dt = new DataTable();

            /* Column names */
            dt.Columns.Add("#", typeof(string));
            dt.Columns.Add("Data", typeof(DateTime));
            dt.Columns.Add("Ora", typeof(string));
            dt.Columns.Add("Log", typeof(string));

            /* Build the query */
            string query = string.Empty;

            /* Read data from SQLite */
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Compress=True;", Database)))
            {
                connection.Open();
                try
                {
                    query = String.Format("SELECT data, strftime('%d.%m.%Y', datetime(data, 'unixepoch', 'localtime')) AS ziua, strftime('%H:%M:%S', datetime(data, 'unixepoch', 'localtime')) AS ora, msg FROM logs ORDER BY id DESC LIMIT 0, {0}", max);
                    using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                    {
                        IDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            object[] row = new object[reader.FieldCount];
                            reader.GetValues(row);

                            /* Fix DateTime cell data */
                            row[1] = DateTime.ParseExact(row[1].ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture);

                            dt.Rows.Add(row);
                        }
                        reader.Close();
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(String.Format("Exception: {0}\r\n | Stack trace: {1}", err.Message, err.StackTrace));
                    Logs.NewRecord(String.Format("Exception: {0}\r\n | Stack trace: {1}", err.Message, err.StackTrace));
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
                connection.Dispose();
            }

            /* Add row number  */
            for (int x = 0; x < dt.Rows.Count; x++)
                dt.Rows[x]["#"] = x + 1;

            return dt;
        }

        /* play */
        public static DataSet LoadGridData()
        {
            DataSet ds = new DataSet();

            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Compress=True;", Database)))
            {
                connection.Open();

                string query = "select * from incarcare";
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
                {
                    adapter.Fill(ds);
                }

                connection.Close();
            }
            return ds;
        }
    }
}
