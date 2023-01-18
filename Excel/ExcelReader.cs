using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using System.Data;
using System.Data.OleDb;
using System.IO;
using ExcelLibrary.SpreadSheet;

namespace Utility.Excel
{
    public class ExcelReader
    {
        public static bool Is2003(string filePath)
        {
            return filePath.ToLower().EndsWith(".xls");
        }

        public static ExcelPackage Read2003(string filePath)
        {
            DataSet ds = GetExcelToDataSet(filePath);

            if (ds == null)
            {
                return null;
            }

            ExcelPackage pck = new ExcelPackage();

            foreach(DataTable dt in ds.Tables)
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(dt.TableName);
                ws.Cells["A1"].LoadFromDataTable(dt, true);
            }

            return pck;
        }

        private static DataSet GetExcelToDataSet(string filePath)
        {
            var connStr = string.Empty;
            if (Path.GetExtension(filePath) == ".xls")
            {
                connStr = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            }
            else if (Path.GetExtension(filePath) == ".xlsx")
            {
                connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';";
            }

            if (string.IsNullOrEmpty(connStr))
            {
                return null;
            }

            List<string> sheetNames = GetSheetNames(filePath);

            var ds = new DataSet();
            var conn = new OleDbConnection(connStr);
            try
            {
                conn.Open();

                OleDbDataAdapter adapter = new OleDbDataAdapter();
                foreach (string sheetName in sheetNames)
                {
                    string query = "Select * From ['" + sheetName + "$']";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds, sheetName);
                }

                conn.Close();
            }
            catch
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                return null;
            }
            finally
            {
                conn.Dispose();
            }

            return ds;
        }

        private static List<string> GetSheetNames(string filePath)
        {
            List<string> sheetNames = new List<string>();
            using(FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                Workbook workbook = Workbook.Load(fileStream);

                foreach(Worksheet sheet in workbook.Worksheets)
                {
                    sheetNames.Add(sheet.Name);
                }
            }

            return sheetNames;
        }
    }
}
