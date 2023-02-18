using System;
using System.IO;
using System.Data;
using System.Globalization;

/* 3rd party libs */
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Linq;

namespace Monitorizare
{
    class Excel
    {
        /* Write data to Excel file */
        public static void WriteSheets(DataTable incarcare, DataTable descarcare)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            var excelFile = String.Format("{0}/Export.xls", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            using (var fs = new FileStream(excelFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                WriteSheet(workbook, "Incarcare", incarcare);
                WriteSheet(workbook, "Descarcare", descarcare);
                workbook.Write(fs);
            }
        }

        /* Generate excel sheet */
        private static HSSFWorkbook WriteSheet(HSSFWorkbook workbook, string title, DataTable dt)
        {
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "Calibri";

            /* headers */
            ISheet sheet = workbook.CreateSheet(title);

            try
            {
                var headRow = sheet.CreateRow(0);

                int i = 0;
                //foreach (string header in headers)
                foreach (DataColumn column in dt.Columns)
                {
                    var cell = headRow.CreateCell(i, CellType.String);
                    cell.SetCellValue(column.ColumnName);
                    cell.CellStyle.SetFont(font);
                    cell.CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    i++;
                }

                /* data */
                int no = 1;
                int rc = dt.Rows.Count;
                int cc = dt.Columns.Count;

                for (int x = 0; x < rc; x++)
                {
                    IRow bodyRow = sheet.CreateRow(no);
                    for (int y = 0; y < cc; y++)
                    {
                        var type = dt.Columns[y].DataType.Name.ToString();

                        bool isNumeric = int.TryParse(dt.Rows[x][y].ToString(), out int value);
                        bool isDateTime = DateTime.TryParse(dt.Rows[x][y].ToString(), out DateTime date);

                        if (type == "Int16" || isNumeric)
                        {
                            var cell = bodyRow.CreateCell(y, CellType.Numeric);
                            cell.SetCellValue(value);
                            cell.CellStyle.SetFont(font);
                        }
                        else if (type == "DateTime")
                        {
                            var cell = bodyRow.CreateCell(y, CellType.String);
                            cell.SetCellValue(date.ToString("dd.MM.yyyy"));
                            cell.CellStyle.SetFont(font);
                        }
                        else
                        {
                            var text = dt.Rows[x][y].ToString();
                            var cell = bodyRow.CreateCell(y, CellType.String);
                            cell.SetCellValue(text);
                            cell.CellStyle.SetFont(font);
                        }
                    }
                    no++;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Exception: {0}\r\n | Stack trace: {1}", err.Message, err.StackTrace);
            }
            finally
            {
                sheet.SetColumnWidth(0, 1500);
                sheet.SetColumnWidth(1, 2600);
                dt.Dispose();
            }
            return workbook;
        }

        /* Export current DataGridView rows */
        public static void ExportTable(DataTable dt, string name)
        {
            string[] headers = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();

            HSSFWorkbook workbook = new HSSFWorkbook();
            var excelFile = String.Format("{0}/Export.xls", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            using (var fs = new FileStream(excelFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                string dateMin = string.Empty;
                string dateMax = string.Empty;
                WriteSheet(workbook, name, dt);
                workbook.Write(fs);
            }
        }
    }
}